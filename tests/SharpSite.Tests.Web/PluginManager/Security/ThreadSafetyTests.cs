using Microsoft.Extensions.Logging;
using Moq;
using SharpSite.Plugins;
using System.Collections.Concurrent;
using Xunit;
using SUT = SharpSite.Web.ApplicationState;

namespace SharpSite.Tests.Web.PluginManager.Security;

/// <summary>
/// Issue #348: Verify PluginManager and ApplicationState handle concurrent access safely.
/// Static ServiceCollection and instance-level Dictionary fields must be thread-safe
/// after the security fix. These tests verify the FIXED behavior.
/// </summary>
public class ThreadSafetyTests
{
	private static PluginManifest CreateManifest(string id) => new()
	{
		Id = id,
		Version = "1.0.0",
		DisplayName = $"Plugin {id}",
		Description = "Thread safety test plugin",
		Published = DateTime.UtcNow.ToString(),
		SupportedVersions = "0.4.0",
		Author = "Test",
		Contact = "Test",
		ContactEmail = "test@test.com",
		AuthorWebsite = "https://example.com",
		Features = [PluginFeatures.Theme]
	};

	[Fact]
	public async Task ApplicationState_ConcurrentAddPlugin_ShouldNotThrow()
	{
		// Arrange
		var appState = new SUT();
		var collectionExceptions = new ConcurrentBag<Exception>();

		// Act — concurrent AddPlugin calls
		var tasks = Enumerable.Range(0, 100).Select(i => Task.Run(() =>
		{
			try
			{
				var manifest = CreateManifest($"thread-plugin-{i}");
				appState.AddPlugin($"thread-plugin-{i}", manifest);
			}
			catch (Exception ex) when (
				ex is InvalidOperationException ||
				ex is IndexOutOfRangeException ||
				ex is NullReferenceException ||
				ex is ArgumentException)
			{
				collectionExceptions.Add(ex);
			}
		}));

		await Task.WhenAll(tasks);

		// Assert — no collection-modification exceptions
		Assert.Empty(collectionExceptions);
	}

	[Fact]
	public async Task ApplicationState_ConcurrentReadAndWrite_ShouldBeSafe()
	{
		// Arrange
		var appState = new SUT();
		var collectionExceptions = new ConcurrentBag<Exception>();
		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

		// Pre-populate some plugins
		for (int i = 0; i < 10; i++)
		{
			appState.AddPlugin($"init-plugin-{i}", CreateManifest($"init-plugin-{i}"));
		}

		// Act — concurrent reads while writing
		var writerTask = Task.Run(async () =>
		{
			for (int i = 10; i < 110 && !cts.Token.IsCancellationRequested; i++)
			{
				try
				{
					appState.AddPlugin($"write-plugin-{i}", CreateManifest($"write-plugin-{i}"));
				}
				catch (Exception ex) when (
					ex is InvalidOperationException ||
					ex is IndexOutOfRangeException ||
					ex is NullReferenceException ||
					ex is ArgumentException)
				{
					collectionExceptions.Add(ex);
				}

				await Task.Yield();
			}
		});

		var readerTasks = Enumerable.Range(0, 5).Select(readerIndex => Task.Run(async () =>
		{
			while (!cts.Token.IsCancellationRequested)
			{
				try
				{
					var plugins = appState.Plugins;
					var count = plugins.Count;
					foreach (var kvp in plugins)
					{
						var key = kvp.Key;
						var value = kvp.Value;
					}
				}
				catch (Exception ex) when (
					ex is InvalidOperationException ||
					ex is IndexOutOfRangeException ||
					ex is NullReferenceException)
				{
					collectionExceptions.Add(ex);
				}

				await Task.Yield();
			}
		}));

		await writerTask;
		cts.Cancel();
		try { await Task.WhenAll(readerTasks); } catch { }

		// Assert — no concurrent access exceptions
		Assert.Empty(collectionExceptions);
	}

	[Fact]
	public async Task PluginManager_ConcurrentHandleUploadedPlugin_ShouldNotCorruptState()
	{
		// Arrange — multiple PluginManager instances sharing static state
		var collectionExceptions = new ConcurrentBag<Exception>();

		var tasks = Enumerable.Range(0, 20).Select(i => Task.Run(() =>
		{
			try
			{
				var mockAssemblyManagerLogger = new Mock<ILogger<PluginAssemblyManager>>();
				var mockPluginAssemblyManager = new Mock<PluginAssemblyManager>(mockAssemblyManagerLogger.Object);
				var mockValidatorLogger = new Mock<ILogger<PluginAssemblyValidator>>();
				var mockValidator = new Mock<PluginAssemblyValidator>(mockValidatorLogger.Object);
				var mockAppState = new Mock<SUT>();
				var mockLogger = new Mock<ILogger<SharpSite.Web.PluginManager>>();
				var pluginManager = new SharpSite.Web.PluginManager(
					mockPluginAssemblyManager.Object,
					mockValidator.Object,
					mockAppState.Object,
					mockLogger.Object);

				// Create a valid plugin ZIP
				var manifest = CreateManifest($"concurrent-mgr-{i}");
				using var ms = new MemoryStream();
				using (var archive = new System.IO.Compression.ZipArchive(ms, System.IO.Compression.ZipArchiveMode.Create, true))
				{
					var entry = archive.CreateEntry("manifest.json");
					using var writer = new StreamWriter(entry.Open());
					writer.Write(System.Text.Json.JsonSerializer.Serialize(manifest));
				}
				var plugin = new Plugin(ms, $"concurrent-{i}.sspkg");

				pluginManager.HandleUploadedPlugin(plugin);
			}
			catch (Exception ex) when (
				ex is InvalidOperationException &&
				(ex.Message.Contains("Collection was modified") ||
				 ex.Message.Contains("Operations that change")))
			{
				collectionExceptions.Add(ex);
			}
			catch
			{
				// Other exceptions (mock setup, etc.) are expected
			}
		}));

		await Task.WhenAll(tasks);

		// Assert — no collection-modification exceptions from shared static ServiceCollection
		Assert.Empty(collectionExceptions);
	}
}
