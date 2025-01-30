using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace SharpSite.Tests.Web.ApplicationState.Load;

public class WhenFileDoesNotExist : BaseFixture
{

	[Fact]
	public async Task ShouldNotInitialize()
	{
		// Arrange
		var services = new ServiceCollection();
		var hubOptions = Options.Create(new HubOptions());
		services.AddSingleton(hubOptions);
		var serviceProvider = services.BuildServiceProvider();

		// Act
		await ApplicationState.Load(serviceProvider, () => string.Empty);

		// Assert
		Assert.False(ApplicationState.Initialized);

	}
}