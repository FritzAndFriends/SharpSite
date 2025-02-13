using SharpSite.Web;
using Xunit;
using WEB = SharpSite.Web;

namespace SharpSite.Tests.Web.RobotsTxt;

public class GenerateRobotsTxt
{
	[Fact]
	public void ShouldIncludeDisallowAdmin()
	{
		// Arrange
		var urlBase = "http://example.com";
		var appState = new WEB.ApplicationState();

		// Act
		var result = Program_RobotsTxt.GenerateRobotsTxt(urlBase, appState);

		// Assert
		Assert.Contains("Disallow: /admin/", result);
	}

	[Fact]
	public void ShouldIncludeSitemap()
	{
		// Arrange
		var urlBase = "http://example.com";
		var appState = new WEB.ApplicationState();

		// Act
		var result = Program_RobotsTxt.GenerateRobotsTxt(urlBase, appState);

		// Assert
		Assert.Contains($"Sitemap: {urlBase}/sitemap.xml", result);
	}

	[Fact]
	public void ShouldIncludeCustomContent()
	{
		// Arrange
		var urlBase = "http://example.com";
		var appState = new WEB.ApplicationState
		{
			RobotsTxtCustomContent = "Custom content"
		};

		// Act
		var result = Program_RobotsTxt.GenerateRobotsTxt(urlBase, appState);

		// Assert
		Assert.Contains("Custom content", result);
	}

	[Fact]
	public void ShouldNotIncludeCustomContent_WhenEmpty()
	{
		// Arrange
		var urlBase = "http://example.com";
		var appState = new WEB.ApplicationState
		{
			RobotsTxtCustomContent = string.Empty
		};

		// Act
		var result = Program_RobotsTxt.GenerateRobotsTxt(urlBase, appState);

		// Assert
		Assert.DoesNotContain("Custom content", result);
	}
}