using Microsoft.Extensions.Logging;
using Moq;
using SharpSite.Plugins;
using System.Collections.Concurrent;

namespace SharpSite.Tests.Plugins;

/// <summary>
/// Issue #348: Verify PluginAssemblyManager handles concurrent access safely.
/// The underlying Dictionary is not thread-safe; after the fix, concurrent
/// AddAssembly/RemoveAssembly calls should not throw collection-modified exceptions.
/// These tests verify the FIXED behavior.
/// </summary>
public class ConcurrentAccessTests
{
	private static PluginManifest CreateManifest(string id) => new()
	{
		Id = id,
		Version = "1.0.0",
		DisplayName = $"Plugin {id}",
		Description = "Concurrent access test plugin",
		Published = DateTime.UtcNow.ToString(),
		SupportedVersions = "0.4.0",
		Author = "Test",
		Contact = "Test",
		ContactEmail = "test@test.com",
		AuthorWebsite = "https://example.com",
		Features = [PluginFeatures.Theme]
	};

	private static Plugin CreateFakePlugin()
	{
		return new Plugin(new MemoryStream(new byte[] { 0x00 }), "fake.sspkg");
	}

	[Fact]
	public async Task ConcurrentAddAssembly_ShouldNotThrow_CollectionModifiedException()
	{
		// Arrange
		var logger = new Mock<ILogger<PluginAssemblyManager>>();
		var manager = new PluginAssemblyManager(logger.Object);
		var collectionExceptions = new ConcurrentBag<Exception>();

		// Act — launch 50 concurrent AddAssembly calls with unique IDs
		var tasks = Enumerable.Range(0, 50).Select(i => Task.Run(() =>
		{
			try
			{
				var manifest = CreateManifest($"concurrent-plugin-{i}");
				var plugin = CreateFakePlugin();
				var assembly = new PluginAssembly(manifest, plugin);
				manager.AddAssembly(assembly);
			}
			catch (InvalidOperationException ex)
			{
				// This is the thread-safety bug: "Collection was modified during enumeration"
				collectionExceptions.Add(ex);
			}
			catch (BadImageFormatException)
			{
				// Expected — fake plugin bytes can't be loaded as a real assembly
			}
			catch (Exception ex) when (
				ex.Message.Contains("Collection was modified") ||
				ex.Message.Contains("index") ||
				ex is IndexOutOfRangeException ||
				ex is NullReferenceException)
			{
				// Dictionary corruption from concurrent access
				collectionExceptions.Add(ex);
			}
			catch
			{
				// Other exceptions (assembly load failures) are expected in test context
			}
		}));

		await Task.WhenAll(tasks);

		// Assert — zero collection-modification exceptions
		Assert.Empty(collectionExceptions);
	}

	[Fact]
	public async Task ConcurrentAddAndRemove_ShouldNotCorruptState()
	{
		// Arrange
		var logger = new Mock<ILogger<PluginAssemblyManager>>();
		var manager = new PluginAssemblyManager(logger.Object);
		var collectionExceptions = new ConcurrentBag<Exception>();

		// Pre-populate with some assemblies
		for (int i = 0; i < 10; i++)
		{
			try
			{
				var manifest = CreateManifest($"prepop-{i}");
				var plugin = CreateFakePlugin();
				var assembly = new PluginAssembly(manifest, plugin);
				manager.AddAssembly(assembly);
			}
			catch (BadImageFormatException) { }
			catch { }
		}

		// Act — concurrent adds and removes
		var addTasks = Enumerable.Range(10, 40).Select(i => Task.Run(() =>
		{
			try
			{
				var manifest = CreateManifest($"prepop-{i}");
				var plugin = CreateFakePlugin();
				var assembly = new PluginAssembly(manifest, plugin);
				manager.AddAssembly(assembly);
			}
			catch (InvalidOperationException ex)
			{
				collectionExceptions.Add(ex);
			}
			catch (BadImageFormatException) { }
			catch (Exception ex) when (
				ex is IndexOutOfRangeException ||
				ex is NullReferenceException ||
				ex.Message.Contains("Collection was modified"))
			{
				collectionExceptions.Add(ex);
			}
			catch { }
		}));

		var removeTasks = Enumerable.Range(0, 10).Select(i => Task.Run(() =>
		{
			try
			{
				var manifest = CreateManifest($"prepop-{i}");
				var plugin = CreateFakePlugin();
				var assembly = new PluginAssembly(manifest, plugin);
				manager.RemoveAssembly(assembly);
			}
			catch (InvalidOperationException ex)
			{
				collectionExceptions.Add(ex);
			}
			catch (Exception ex) when (
				ex is IndexOutOfRangeException ||
				ex is NullReferenceException ||
				ex.Message.Contains("Collection was modified"))
			{
				collectionExceptions.Add(ex);
			}
			catch { }
		}));

		await Task.WhenAll(addTasks.Concat(removeTasks));

		// Assert — no concurrent access exceptions
		Assert.Empty(collectionExceptions);
	}

	[Fact]
	public async Task ConcurrentReadWhileWriting_ShouldBeSafe()
	{
		// Arrange
		var logger = new Mock<ILogger<PluginAssemblyManager>>();
		var manager = new PluginAssemblyManager(logger.Object);
		var collectionExceptions = new ConcurrentBag<Exception>();
		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

		// Act — concurrent reads (via Assemblies property) while writing
		var writerTask = Task.Run(async () =>
		{
			for (int i = 0; i < 100 && !cts.Token.IsCancellationRequested; i++)
			{
				try
				{
					var manifest = CreateManifest($"rw-plugin-{i}");
					var plugin = CreateFakePlugin();
					var assembly = new PluginAssembly(manifest, plugin);
					manager.AddAssembly(assembly);
				}
				catch (BadImageFormatException) { }
				catch (InvalidOperationException ex)
				{
					collectionExceptions.Add(ex);
				}
				catch (Exception ex) when (
					ex is IndexOutOfRangeException ||
					ex is NullReferenceException ||
					ex.Message.Contains("Collection was modified"))
				{
					collectionExceptions.Add(ex);
				}
				catch { }

				await Task.Yield();
			}
		});

		var readerTasks = Enumerable.Range(0, 5).Select(readerIndex => Task.Run(async () =>
		{
			while (!cts.Token.IsCancellationRequested)
			{
				try
				{
					// Enumerate the Assemblies collection while writer is active
					var count = manager.Assemblies.Count;
					foreach (var kvp in manager.Assemblies)
					{
						var key = kvp.Key;
						var value = kvp.Value;
					}
				}
				catch (InvalidOperationException ex)
				{
					collectionExceptions.Add(ex);
				}
				catch (Exception ex) when (
					ex is IndexOutOfRangeException ||
					ex is NullReferenceException)
				{
					collectionExceptions.Add(ex);
				}
				catch { }

				await Task.Yield();
			}
		}));

		await writerTask;
		cts.Cancel();
		try { await Task.WhenAll(readerTasks); } catch { }

		// Assert — no collection-modified exceptions from concurrent read/write
		Assert.Empty(collectionExceptions);
	}
}
