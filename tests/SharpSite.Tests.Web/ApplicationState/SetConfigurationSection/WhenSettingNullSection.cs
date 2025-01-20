using Xunit;


namespace SharpSite.Tests.Web.ApplicationState.SetConfigurationSection;

public class WhenSettingNullSection : BaseFixture
{

	[Fact]
	public void ThenThrowsArgumentNullException()
	{
		// Act
		Action act = () => ApplicationState.SetConfigurationSection(null!);

		// Assert
		Assert.Throws<ArgumentNullException>(act);

	}
}
