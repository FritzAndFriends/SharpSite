using Xunit;


namespace SharpSite.Tests.Web.ApplicationState.SetConfigurationSection;

public class WhenSettingNullSection : BaseFixture
{
    [Fact]
    public async Task ThenThrowsArgumentNullException()
    {
        // Act
        async Task Act() => await ApplicationState.SetConfigurationSection(null!);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Act);
    }
}
