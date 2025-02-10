using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Xunit;
using SUT = SharpSite.Web.ApplicationState;

namespace SharpSite.Tests.Web.ApplicationState.Load;

public class WhenFileExists : BaseFixture
{
	[Fact]
	public async Task ShouldInitializeState()
	{
		// Arrange
		var services = new ServiceCollection();
		var hubOptions = Options.Create(new HubOptions());
		services.AddSingleton(hubOptions);
		var serviceProvider = services.BuildServiceProvider();

		var state = new SUT
		{
			MaximumUploadSizeMB = 20,
			PageNotFoundContent = "Not Found",
			Localization = new SUT.LocalizationRecord("en-US", new[] { "en-US", "fr-FR" }),
			CurrentTheme = new SUT.CurrentThemeRecord("theme-v1")
		};

		var json = JsonConvert.SerializeObject(state, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });

		// Act
		await ApplicationState.Load(serviceProvider, () => json);

		// Assert
		Assert.True(ApplicationState.Initialized);
		Assert.Equal(20, ApplicationState.MaximumUploadSizeMB);
		Assert.Equal("Not Found", ApplicationState.PageNotFoundContent);
		Assert.Equal("en-US", ApplicationState.Localization?.DefaultCulture);
		Assert.Equal(new[] { "en-US", "fr-FR" }, ApplicationState.Localization?.SupportedCultures);
		Assert.Equal("theme-v1", ApplicationState.CurrentTheme?.IdVersion);
	}

}
