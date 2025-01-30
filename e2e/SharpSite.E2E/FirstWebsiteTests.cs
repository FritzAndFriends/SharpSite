using Microsoft.Playwright;

namespace SharpSite.E2E;

public class FirstWebsiteTests : SharpSitePageTest
{

	[Fact]
	public async Task HasAboutSharpSiteLink()
	{
		await Page.GotoAsync("/");
		// Click the get started link.
		await Page.GetByRole(AriaRole.Link, new() { Name = "About SharpSite" }).ClickAsync();

		// take a screenshot
		await Page.ScreenshotAsync(new PageScreenshotOptions() { Path = "about-sharpsite.png" });

		// Expects page to have a heading with the name of Installation.
		await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "About SharpSite" })).ToBeVisibleAsync();
	}

}
