using Moq;
using SharpSite.Abstractions.Base;
using Xunit;


namespace SharpSite.Tests.Web.ApplicationState.SetConfigurationSection;

public class WhenAddingNewSection : BaseFixture
{
	[Fact]
	public void ThenSectionIsAdded()
	{
		// Arrange
		var sectionMock = new Mock<ISharpSiteConfigurationSection>();
		sectionMock.Setup(s => s.SectionName).Returns("TestSection");
		var section = sectionMock.Object;

		// Act
		ApplicationState.SetConfigurationSection(section);

		// Assert
		Assert.Contains(section, ApplicationState.ConfigurationSections.Values);
	}

	[Fact]
	public void ThenEventHandlerIsTriggered()
	{
		// Arrange
		var sectionMock = new Mock<ISharpSiteConfigurationSection>();
		sectionMock.Setup(s => s.SectionName).Returns("TestSection");
		var section = sectionMock.Object;
		var eventHandlerTriggered = false;
		ApplicationState.ConfigurationSectionChanged += (sender, args) => eventHandlerTriggered = true;

		// Act
		ApplicationState.SetConfigurationSection(section);

		// Assert
		Assert.True(eventHandlerTriggered);

	}
}
