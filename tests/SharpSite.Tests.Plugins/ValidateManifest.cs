using Microsoft.Extensions.Logging;
using Moq;
using SharpSite.Plugins;


namespace SharpSite.Tests.Plugins;

public class ValidateManifest
{
	private readonly Mock<ILogger> _loggerMock;
	private readonly Mock<Plugin> _pluginMock;
	private const string _PluginName = "Test Plugin";

	public ValidateManifest()
	{
		_loggerMock = new Mock<ILogger>();
		_pluginMock = new Mock<Plugin>(new MemoryStream(), _PluginName);
	}

	[Fact]
	public void ValidManifestSucceeds()
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

		// Act
		manifest.ValidateManifest(_loggerMock.Object, _pluginMock.Object);

		// Assert
		_loggerMock.VerifyNoOtherCalls();
	}

	[Fact]
	public void EmptyIdShouldThrow()
	{
		// Arrange
		var manifest = new PluginManifest
		{
			Id = string.Empty,
			Version = "1.0.0",
			DisplayName = "Test Plugin",
			Description = "Test plugin description",
			Published = DateTime.UtcNow.ToString(),
			SupportedVersions = "0.4.0",
			Author = "Test Author",
			Contact = "Test Contact",
			ContactEmail = "test@example.com",
			AuthorWebsite = "https://example.com",
			Features = [PluginFeatures.Theme]
		};

		// Act & Assert
		Assert.Throws<PluginException>(() =>
			manifest.ValidateManifest(_loggerMock.Object, _pluginMock.Object));
		_loggerMock.Verify(x => x.Log(
			LogLevel.Error,
			It.IsAny<EventId>(),
			It.Is<It.IsAnyType>((v, t) => string.Equals($"Invalid plugin manifest: {_PluginName}", v.ToString())),
			null,
			It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
	}

	[Theory]
	[InlineData("plugin$id")]
	[InlineData("plugin@id")]
	[InlineData("plugin id")]
	public void InvalidIdShouldThrow(string id)
	{
		// Arrange
		var manifest = new PluginManifest
		{
			Id = id,
			Version = "1.0.0",
			DisplayName = "Test Plugin",
			Description = "Test plugin description",
			Published = DateTime.UtcNow.ToString(),
			SupportedVersions = "0.4.0",
			Author = "Test Author",
			Contact = "Test Contact",
			ContactEmail = "test@example.com",
			AuthorWebsite = "https://example.com",
			Features = [PluginFeatures.Theme]
		};

		// Act & Assert
		Assert.Throws<PluginException>(() =>
			manifest.ValidateManifest(_loggerMock.Object, _pluginMock.Object));
		_loggerMock.Verify(x => x.Log(
			LogLevel.Error,
			It.IsAny<EventId>(),
			It.Is<It.IsAnyType>((v, t) => string.Equals($"Invalid plugin manifest: {_PluginName}", v.ToString())),
			null,
			It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
	}

	[Theory]
	[InlineData("1.0$0")]
	[InlineData("1.0@0")]
	[InlineData("1.0 0")]
	public void InvalidVersionShouldThrow(string version)
	{
		// Arrange
		var manifest = new PluginManifest
		{
			Id = "test-plugin",
			Version = version,
			DisplayName = "Test Plugin",
			Description = "Test plugin description",
			Published = DateTime.UtcNow.ToString(),
			SupportedVersions = "0.4.0",
			Author = "Test Author",
			Contact = "Test Contact",
			ContactEmail = "test@example.com",
			AuthorWebsite = "https://example.com",
			Features = [PluginFeatures.Theme]
		};

		// Act & Assert
		Assert.Throws<PluginException>(() =>
			manifest.ValidateManifest(_loggerMock.Object, _pluginMock.Object));
		_loggerMock.Verify(x => x.Log(
			LogLevel.Error,
			It.IsAny<EventId>(),
			It.Is<It.IsAnyType>((v, t) => string.Equals($"Invalid plugin manifest: {_PluginName}", v.ToString())),
			null,
			It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
	}

	[Fact]
	public void EmptyDisplayNameShouldThrow()
	{
		// Arrange
		var manifest = new PluginManifest
		{
			Id = "test-plugin",
			DisplayName = string.Empty,
			Version = "1.0.0",
			Description = "Test plugin description",
			Published = DateTime.UtcNow.ToString(),
			SupportedVersions = "0.4.0",
			Author = "Test Author",
			Contact = "Test Contact",
			ContactEmail = "test@example.com",
			AuthorWebsite = "https://example.com",
			Features = [PluginFeatures.Theme]
		};

		// Act & Assert
		Assert.Throws<PluginException>(() =>
			manifest.ValidateManifest(_loggerMock.Object, _pluginMock.Object));
		_loggerMock.Verify(x => x.Log(
			LogLevel.Error,
			It.IsAny<EventId>(),
			It.Is<It.IsAnyType>((v, t) => string.Equals($"Invalid plugin manifest: {_PluginName}", v.ToString())),
			null,
			It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
	}
}
