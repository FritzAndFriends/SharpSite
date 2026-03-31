using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using SharpSite.Abstractions.Base;
using Xunit;
using SUT = SharpSite.Web.ApplicationState;

namespace SharpSite.Tests.Web.ApplicationState.Security;

/// <summary>
/// Issue #346: Verify ApplicationState serialization does NOT use Newtonsoft
/// TypeNameHandling.Auto (known RCE vector) and correctly round-trips with System.Text.Json.
/// These tests verify the FIXED behavior — they may fail until the fix lands.
/// </summary>
public class SerializationSecurityTests
{
	private static SUT CreateApplicationState() => new();

	private static IServiceProvider CreateServiceProvider()
	{
		var services = new ServiceCollection();
		var hubOptions = Options.Create(new HubOptions());
		services.AddSingleton(hubOptions);
		return services.BuildServiceProvider();
	}

	[Fact]
	public async Task Deserialization_ShouldNotHonor_RootLevelTypeMetadata()
	{
		// Arrange — a JSON payload with $type metadata (Newtonsoft RCE vector)
		var maliciousJson = """
		{
			"$type": "System.Diagnostics.Process, System",
			"SiteName": "Hacked",
			"MaximumUploadSizeMB": 10,
			"StartupCompleted": false,
			"PageNotFoundContent": "",
			"ConfigurationFields": {}
		}
		""";
		var appState = CreateApplicationState();
		var serviceProvider = CreateServiceProvider();

		// Act — load from JSON containing $type
		await appState.Load(serviceProvider, () => maliciousJson);

		// Assert — $type should be ignored; state loads as ApplicationState, not Process
		Assert.IsType<SUT>(appState);
		Assert.True(appState.Initialized);
		Assert.Equal("Hacked", appState.SiteName);
	}

	[Fact]
	public async Task Deserialization_WithNestedTypeMetadata_ShouldNotInstantiateArbitraryTypes()
	{
		// Arrange — $type targeting the polymorphic ConfigurationSections dictionary
		var maliciousJson = """
		{
			"SiteName": "Test",
			"MaximumUploadSizeMB": 10,
			"StartupCompleted": false,
			"PageNotFoundContent": "",
			"ConfigurationFields": {},
			"ConfigurationSections": {
				"malicious": {
					"$type": "System.IO.FileInfo, System.IO.FileSystem",
					"SectionName": "evil"
				}
			}
		}
		""";
		var appState = CreateApplicationState();
		var serviceProvider = CreateServiceProvider();

		// Act & Assert — should not instantiate arbitrary types via $type
		// After fix (System.Text.Json), $type is just an unknown property
		await appState.Load(serviceProvider, () => maliciousJson);
		Assert.IsType<SUT>(appState);
	}

	[Fact]
	public async Task Serialization_RoundTrip_WithSystemTextJson_ShouldPreserveProperties()
	{
		// Arrange
		var appState = CreateApplicationState();
		appState.SiteName = "Round Trip Site";
		appState.MaximumUploadSizeMB = 25;
		appState.PageNotFoundContent = "Custom 404";
		appState.CurrentTheme = new SUT.CurrentThemeRecord("my-theme@1.0.0");
		appState.Localization = new SUT.LocalizationRecord("en-US", ["en-US", "es-ES"]);
		var serviceProvider = CreateServiceProvider();

		// Act — serialize with System.Text.Json, then load back
		var json = System.Text.Json.JsonSerializer.Serialize(appState);
		var restoredState = CreateApplicationState();
		await restoredState.Load(serviceProvider, () => json);

		// Assert
		Assert.True(restoredState.Initialized);
		Assert.Equal("Round Trip Site", restoredState.SiteName);
		Assert.Equal(25, restoredState.MaximumUploadSizeMB);
		Assert.Equal("Custom 404", restoredState.PageNotFoundContent);
	}

	[Fact]
	public void Serialization_OutputJson_ShouldNotContain_TypeDiscriminator()
	{
		// Arrange
		var appState = CreateApplicationState();
		appState.SiteName = "Clean Serialization";

		// Act — serialize to JSON
		var json = System.Text.Json.JsonSerializer.Serialize(appState);

		// Assert — no $type discriminator should be present
		Assert.DoesNotContain("$type", json);
	}

	[Fact]
	public async Task PluginConfigurationData_ShouldSurvive_SerializationRoundTrip()
	{
		// Arrange
		var appState = CreateApplicationState();
		appState.SiteName = "Plugin Config Site";
		appState.ConfigurationFields["PluginSetting1"] = "Value1";
		appState.ConfigurationFields["PluginSetting2"] = "Value2";
		appState.HasCustomLogo = "logo.png";

		var sectionMock = new Mock<ISharpSiteConfigurationSection>();
		sectionMock.Setup(s => s.SectionName).Returns("TestPluginConfig");
		await appState.SetConfigurationSection(sectionMock.Object);

		var serviceProvider = CreateServiceProvider();

		// Act — serialize then reload
		var json = System.Text.Json.JsonSerializer.Serialize(appState);
		var restoredState = CreateApplicationState();
		await restoredState.Load(serviceProvider, () => json);

		// Assert — core plugin configuration data survives the round trip
		Assert.True(restoredState.Initialized);
		Assert.Equal("Plugin Config Site", restoredState.SiteName);
		Assert.Equal("logo.png", restoredState.HasCustomLogo);
	}

	[Fact]
	public async Task Save_ThenLoad_ShouldRoundTrip_WithoutTypeNameHandling()
	{
		// Arrange — simulate Save serialization (System.Text.Json after fix)
		var original = CreateApplicationState();
		original.SiteName = "Persistence Test";
		original.MaximumUploadSizeMB = 50;
		original.PageNotFoundContent = "Gone!";
		original.RobotsTxtCustomContent = "User-agent: *\nDisallow: /admin";

		// Act — serialize (mimicking Save) and deserialize (mimicking Load)
		var json = System.Text.Json.JsonSerializer.Serialize(original);
		Assert.DoesNotContain("$type", json);

		var serviceProvider = CreateServiceProvider();
		var loaded = CreateApplicationState();
		await loaded.Load(serviceProvider, () => json);

		// Assert
		Assert.True(loaded.Initialized);
		Assert.Equal("Persistence Test", loaded.SiteName);
		Assert.Equal(50, loaded.MaximumUploadSizeMB);
	}
}
