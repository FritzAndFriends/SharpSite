using Moq;
using SharpSite.Abstractions.Base;
using Xunit;


namespace SharpSite.Tests.Web.ApplicationState.SetConfigurationSection;

public class WhenAddingNewSection : BaseFixture
{
    [Fact]
    public async Task ThenSectionIsAdded()
    {
        // Arrange
        var sectionMock = new Mock<ISharpSiteConfigurationSection>();
        sectionMock.Setup(s => s.SectionName).Returns("TestSection");
        var section = sectionMock.Object;

        // Act
        await ApplicationState.SetConfigurationSection(section);

        // Assert
        Assert.Contains(section, ApplicationState.ConfigurationSections.Values);
    }

    [Fact]
    public async Task ThenEventHandlerIsTriggered()
    {
        // Arrange
        var sectionMock = new Mock<ISharpSiteConfigurationSection>();
        sectionMock.Setup(s => s.SectionName).Returns("TestSection");
        var section = sectionMock.Object;
        var eventHandlerTriggered = false;
        ApplicationState.ConfigurationSectionChanged += async (sender, args) =>
        {
            eventHandlerTriggered = true;
            await Task.CompletedTask;
        };

        // Act
        await ApplicationState.SetConfigurationSection(section);

        // Assert
        Assert.True(eventHandlerTriggered);
    }
}
