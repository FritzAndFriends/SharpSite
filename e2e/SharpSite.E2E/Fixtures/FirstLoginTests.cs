using Microsoft.Playwright;
using SharpSite.E2E.Abstractions;
using Xunit;

namespace SharpSite.E2E.Fixtures;


public class FirstLoginTests : AuthenticatedPageTests
{
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
    public Task CanLogout()
    {
        return Task.CompletedTask;	

        // await LoginAsDefaultAdmin();
        // await Logout();
        // await Page.ScreenshotAsync(new PageScreenshotOptions() { Path = "loggedout.png" });
        // // check for the login link
        // await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Login" })).ToBeVisibleAsync();
    }
}


public class FirstVisitTests : SharpSitePageTest
{

	// add a test that visits the home page and takes a screenshot
	[Fact]
	public async Task CanVisitHomePage()
	{

		await Page.GotoAsync("/");
		await Page.ScreenshotAsync(new PageScreenshotOptions() { Path = "home.png" });
		// check for the login link
		await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Login" })).ToBeVisibleAsync();
	}

}