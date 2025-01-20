using Xunit;


namespace SharpSite.Tests.Web.ApplicationState.SetConfigurationSection;

public class GivenNullSection : BaseFixture
{

	[Fact]
	public void WhenSettingNullSection_ThenThrowsArgumentNullException()
	{
		// Act
		Action act = () => ApplicationState.SetConfigurationSection(null!);

		// Assert
		Assert.Throws<ArgumentNullException>(act);

	}
}