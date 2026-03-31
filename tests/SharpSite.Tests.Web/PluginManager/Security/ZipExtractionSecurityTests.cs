using Microsoft.Extensions.Logging;
using Moq;
using SharpSite.Plugins;
using System.IO.Compression;
using System.Text.Json;
using Xunit;

namespace SharpSite.Tests.Web.PluginManager.Security;

/// <summary>
/// Issue #347: Verify plugin ZIP extraction enforces size limits, compression ratio
/// caps, and path traversal prevention. Tests verify the FIXED behavior —
/// they will fail until River's security fix lands.
/// </summary>
public class ZipExtractionSecurityTests
{
	private readonly SharpSite.Web.PluginManager _PluginManager;
	private readonly Mock<ILogger<SharpSite.Web.PluginManager>> _MockLogger;

	public ZipExtractionSecurityTests()
	{
		var mockAssemblyManagerLogger = new Mock<ILogger<PluginAssemblyManager>>();
		var mockPluginAssemblyManager = new Mock<PluginAssemblyManager>(mockAssemblyManagerLogger.Object);
		var mockValidatorLogger = new Mock<ILogger<PluginAssemblyValidator>>();
		var mockValidator = new Mock<PluginAssemblyValidator>(mockValidatorLogger.Object);
		var mockAppState = new Mock<SharpSite.Web.ApplicationState>();
		_MockLogger = new Mock<ILogger<SharpSite.Web.PluginManager>>();
		_PluginManager = new SharpSite.Web.PluginManager(
			mockPluginAssemblyManager.Object,
			mockValidator.Object,
			mockAppState.Object,
			_MockLogger.Object);
	}

	private static PluginManifest CreateValidManifest(string id = "test-plugin") => new()
	{
		Id = id,
		Version = "1.0.0",
		DisplayName = "Test Plugin",
		Description = "Test plugin for security tests",
		Published = DateTime.UtcNow.ToString(),
		SupportedVersions = "0.4.0",
		Author = "Test Author",
		Contact = "Test Contact",
		ContactEmail = "test@example.com",
		AuthorWebsite = "https://example.com",
		Features = [PluginFeatures.Theme]
	};

	private static byte[] CreateZipWithManifestAndEntries(
		PluginManifest manifest,
		params (string path, byte[] content)[] entries)
	{
		using var memoryStream = new MemoryStream();
		using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
		{
			// Always include a valid manifest
			var manifestEntry = archive.CreateEntry("manifest.json");
			using (var writer = new StreamWriter(manifestEntry.Open()))
			{
				writer.Write(JsonSerializer.Serialize(manifest));
			}

			foreach (var (path, content) in entries)
			{
				var entry = archive.CreateEntry(path);
				using var entryStream = entry.Open();
				entryStream.Write(content, 0, content.Length);
			}
		}

		return memoryStream.ToArray();
	}

	[Fact]
	public void ValidZipFile_ShouldExtractSuccessfully()
	{
		// Arrange — a normal plugin ZIP with valid manifest and small lib file
		var manifest = CreateValidManifest();
		var dllContent = new byte[1024]; // 1KB fake DLL
		Array.Fill<byte>(dllContent, 0x42);

		var zipBytes = CreateZipWithManifestAndEntries(manifest,
			("lib/test-plugin.dll", dllContent));

		using var ms = new MemoryStream(zipBytes);
		var plugin = new Plugin(ms, "test.sspkg");

		// Act — should not throw
		_PluginManager.HandleUploadedPlugin(plugin);

		// Assert
		Assert.NotNull(_PluginManager.Manifest);
		Assert.Equal("test-plugin", _PluginManager.Manifest.Id);
	}

	[Fact]
	public void ExtractPlugin_ShouldReject_SingleFileOverMaxSizeLimit()
	{
		// Arrange — ZIP with a single entry exceeding 50MB per-file limit
		var manifest = CreateValidManifest();
		var oversizedContent = new byte[51 * 1024 * 1024]; // 51MB of zeros

		var zipBytes = CreateZipWithManifestAndEntries(manifest,
			("lib/huge-file.dll", oversizedContent));

		using var ms = new MemoryStream(zipBytes);
		var plugin = new Plugin(ms, "oversized.sspkg");

		// Act & Assert — extraction should reject the oversized entry
		Assert.ThrowsAny<Exception>(() => _PluginManager.HandleUploadedPlugin(plugin));
	}

	[Fact]
	public void ExtractPlugin_ShouldReject_TotalExtractionSizeOverCap()
	{
		// Arrange — ZIP with multiple entries totaling over 100MB
		var manifest = CreateValidManifest();

		// Create 5 entries of 25MB each = 125MB total (over 100MB cap)
		var entries = new (string path, byte[] content)[5];
		for (int i = 0; i < 5; i++)
		{
			var content = new byte[25 * 1024 * 1024]; // 25MB each
			entries[i] = ($"lib/chunk-{i}.dll", content);
		}

		var zipBytes = CreateZipWithManifestAndEntries(manifest, entries);
		using var ms = new MemoryStream(zipBytes);
		var plugin = new Plugin(ms, "total-oversized.sspkg");

		// Act & Assert — extraction should reject when total exceeds cap
		Assert.ThrowsAny<Exception>(() => _PluginManager.HandleUploadedPlugin(plugin));
	}

	[Fact]
	public void ExtractPlugin_ShouldReject_HighCompressionRatio()
	{
		// Arrange — ZIP with extremely high compression ratio (>100:1)
		// A large block of zeros compresses to almost nothing, creating a "zip bomb" pattern
		var manifest = CreateValidManifest();
		var highlyCompressibleContent = new byte[10 * 1024 * 1024]; // 10MB of zeros
		// Zeros compress at roughly 1000:1, well above the 100:1 limit

		var zipBytes = CreateZipWithManifestAndEntries(manifest,
			("lib/bomb.dll", highlyCompressibleContent));

		using var ms = new MemoryStream(zipBytes);
		var plugin = new Plugin(ms, "bomb.sspkg");

		// Act & Assert — compression ratio validation should reject this
		Assert.ThrowsAny<Exception>(() => _PluginManager.HandleUploadedPlugin(plugin));
	}

	[Fact]
	public void ExtractPlugin_ShouldBlock_PathTraversalInFilenames()
	{
		// Arrange — ZIP with path traversal attempt via ../ in entry names
		var manifest = CreateValidManifest();
		var maliciousContent = System.Text.Encoding.UTF8.GetBytes("malicious payload");

		var zipBytes = CreateZipWithManifestAndEntries(manifest,
			("lib/../../../etc/shadow", maliciousContent));

		using var ms = new MemoryStream(zipBytes);
		var plugin = new Plugin(ms, "traversal.sspkg");

		// Act & Assert — path traversal should be blocked
		Assert.ThrowsAny<Exception>(() => _PluginManager.HandleUploadedPlugin(plugin));
	}

	[Fact]
	public void ExtractPlugin_ShouldBlock_PathTraversalWithBackslashes()
	{
		// Arrange — path traversal using Windows-style backslashes
		var manifest = CreateValidManifest();
		var maliciousContent = System.Text.Encoding.UTF8.GetBytes("malicious payload");

		var zipBytes = CreateZipWithManifestAndEntries(manifest,
			("lib\\..\\..\\..\\windows\\system32\\evil.dll", maliciousContent));

		using var ms = new MemoryStream(zipBytes);
		var plugin = new Plugin(ms, "traversal-backslash.sspkg");

		// Act & Assert — path traversal with backslashes should be blocked
		Assert.ThrowsAny<Exception>(() => _PluginManager.HandleUploadedPlugin(plugin));
	}

	[Fact]
	public void ExtractPlugin_ShouldBlock_DotDotInEntryName()
	{
		// Arrange — entry name containing ".." without slashes
		var manifest = CreateValidManifest();
		var content = System.Text.Encoding.UTF8.GetBytes("content");

		var zipBytes = CreateZipWithManifestAndEntries(manifest,
			("lib/..sneaky/payload.dll", content));

		using var ms = new MemoryStream(zipBytes);
		var plugin = new Plugin(ms, "dotdot.sspkg");

		// Act & Assert — any ".." in path should be blocked
		Assert.ThrowsAny<Exception>(() => _PluginManager.HandleUploadedPlugin(plugin));
	}

	[Fact]
	public void ValidZipWithWebContent_ShouldExtractSuccessfully()
	{
		// Arrange — a plugin with both lib and web content
		var manifest = CreateValidManifest();
		var dllContent = new byte[512];
		Array.Fill<byte>(dllContent, 0x42);
		var cssContent = System.Text.Encoding.UTF8.GetBytes("body { color: red; }");

		var zipBytes = CreateZipWithManifestAndEntries(manifest,
			("lib/test-plugin.dll", dllContent),
			("web/styles.css", cssContent));

		using var ms = new MemoryStream(zipBytes);
		var plugin = new Plugin(ms, "webplugin.sspkg");

		// Act — should not throw
		_PluginManager.HandleUploadedPlugin(plugin);

		// Assert
		Assert.NotNull(_PluginManager.Manifest);
		Assert.Equal("test-plugin", _PluginManager.Manifest.Id);
	}
}
