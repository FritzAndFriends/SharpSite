using Microsoft.Extensions.Logging;
using Moq;
using SharpSite.Plugins;
using System.IO.Compression;
using System.Text.Json;
using Xunit;

namespace SharpSite.Tests.Web.PluginManager;

public class HandleUploadedPlugin
{
	private readonly SharpSite.Web.PluginManager _PluginManager;
	private readonly Mock<ILogger<SharpSite.Web.PluginManager>> _MockLogger;

	public HandleUploadedPlugin()
	{
		var mockAssemblyManagerLogger = new Mock<ILogger<PluginAssemblyManager>>();
		var mockPluginAssemblyManager = new Mock<PluginAssemblyManager>(mockAssemblyManagerLogger.Object);
		var mockAppState = new Mock<SharpSite.Web.ApplicationState>();
		_MockLogger = new Mock<ILogger<SharpSite.Web.PluginManager>>();
		_PluginManager = new SharpSite.Web.PluginManager(
			mockPluginAssemblyManager.Object,
			mockAppState.Object,
			_MockLogger.Object);
	}

	[Fact]
	public void ValidPluginSucceeds()
	{
		// Arrange
		var manifest = new PluginManifest 
		{ 
			Id = "test-plugin",
			Version = "1.0.0",
			DisplayName = "Test Plugin Display Name",
			Description = "Test plugin description",
			Published = DateTime.UtcNow.ToString(),
			SupportedVersions = "0.4.0",
			Author = "Test Author",
			Contact = "Test Contact",
			ContactEmail = "test@example.com",
			AuthorWebsite = "https://example.com",
			Features = [PluginFeatures.Theme]
		};
		var manifestJson = JsonSerializer.Serialize(manifest);
		using var memoryStream = new MemoryStream();
		using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
		{
			var manifestEntry = archive.CreateEntry("manifest.json");
			using var writer = new StreamWriter(manifestEntry.Open());
			writer.Write(manifestJson);
		}
		var plugin = new Plugin(memoryStream, "test.sspkg");

		// Act
		_PluginManager.HandleUploadedPlugin(plugin);

		// Assert
		Assert.NotNull(_PluginManager.Manifest);
		Assert.Equal("test-plugin", _PluginManager.Manifest.Id);
		_MockLogger.Verify(x => x.Log(
			LogLevel.Information,
			It.IsAny<EventId>(),
			It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Plugin {manifest} uploaded and manifest processed.")),
			null,
			It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
			Times.Once);
	}

	[Fact]
	public void NullPluginThrowsArgumentNullException()
	{
		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => _PluginManager.HandleUploadedPlugin(null!));
	}

	[Fact]
	public void MissingManifestThrowsException()
	{
		// Arrange
		using var memoryStream = new MemoryStream();
		using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
		{
			var readmeEntry = archive.CreateEntry("README.md");
			using var writer = new StreamWriter(readmeEntry.Open());
			writer.Write("# Test README.md");
		}
		var plugin = new Plugin(memoryStream, "test.sspkg");

		// Act & Assert
		var ex = Assert.Throws<Exception>(() => _PluginManager.HandleUploadedPlugin(plugin));
		Assert.Equal("manifest.json not found in the ZIP file.", ex.Message);
		_MockLogger.Verify(x => x.Log(
			LogLevel.Error,
			It.IsAny<EventId>(),
			It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Manifest file missing in plugin: {plugin.Name}")),
			It.IsAny<Exception>(),
			It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
			Times.Once);
	}

	[Fact]
	public void InvalidManifestThrowsJsonException()
	{
		// Arrange
		using var memoryStream = new MemoryStream();
		using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
		{
			var manifestEntry = archive.CreateEntry("manifest.json");
			using var writer = new StreamWriter(manifestEntry.Open());
			writer.Write("invalid json");
		}
		var plugin = new Plugin(memoryStream, "test.sspkg");

		// Act & Assert
		Assert.Throws<JsonException>(() => _PluginManager.HandleUploadedPlugin(plugin));
	}
}
