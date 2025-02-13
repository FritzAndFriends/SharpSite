using Microsoft.Playwright;

namespace SharpSite.E2E;

public class FirstLoginTests : AuthenticatedPageTests
{


	[Fact]
	public async Task HasLoginLink()
	{
		await Page.GotoAsync("/");
		// Click the get started link.
		await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
		// take a screenshot
		await Page.ScreenshotAsync(new PageScreenshotOptions() { Path = "login.png" });
		// Expects page to have a heading with the name of Installation.
		await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Login" })).ToBeVisibleAsync();
	}

	// add a test that clicks the login link and then logs in
	[Fact]
	public async Task CanLogin()
	{
		await LoginAsDefaultAdmin();
		await Page.ScreenshotAsync(new PageScreenshotOptions() { Path = "loggedin.png" });

		// check for the manage profile link with the text "Site Admin"
		await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Site Admin" })).ToBeVisibleAsync();

	}

	// add a test that logs in and then logs out
	[Fact]
	public async Task CanLogout()
	{
		await LoginAsDefaultAdmin();
		await Logout();
		await Page.ScreenshotAsync(new PageScreenshotOptions() { Path = "loggedout.png" });
		// check for the login link
		await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Login" })).ToBeVisibleAsync();
	}
}
