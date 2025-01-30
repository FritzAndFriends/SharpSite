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

public abstract class AuthenticatedPageTests : SharpSitePageTest
{

	private const string URL_LOGIN = "/Account/Login";
	private const string LOGIN_USERID = "admin@Localhost";
	private const string LOGIN_PASSWORD = "Admin123!";

	protected async Task LoginAsDefaultAdmin()
	{
		await Page.GotoAsync(URL_LOGIN);
		await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
		await Page.GetByRole(AriaRole.Textbox, new() { Name = "Input.Email" })
			.FillAsync(LOGIN_USERID);
		await Page.GetByRole(AriaRole.Textbox, new() { Name = "Input.Password" })
			.FillAsync(LOGIN_PASSWORD);
		await Page.GetByRole(AriaRole.Button, new() { Name = "loginbutton" }).ClickAsync();
	}

	protected async Task Logout()
	{
		await Page.GetByRole(AriaRole.Button, new() { Name = "Logout" }).ClickAsync();
	}

}


public class ProfileTests : AuthenticatedPageTests
{

	[Fact]
	public async Task CanViewProfile()
	{

		await LoginAsDefaultAdmin();

		await Page.GotoAsync("/Account/Manage");
		await Page.ScreenshotAsync(new PageScreenshotOptions() { Path = "profile.png" });
		// check for the manage profile link with the text "Site Admin"
		await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Manage Profile" })).ToBeVisibleAsync();
	}

	[Fact]
	public async Task CanChangePhoneNumber()
	{

		await LoginAsDefaultAdmin();

		// define a testPhoneNumber variable as random string of 10 digits
		var testPhoneNumber = Random.Shared.NextInt64(1000000000, 9999999999).ToString();

		await Page.GetByLabel("Manage Profile").ClickAsync();
		await Page.GetByPlaceholder("Enter your phone number").ClickAsync();
		await Page.GetByPlaceholder("Enter your phone number").FillAsync(testPhoneNumber);
		await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

		await Page.ScreenshotAsync(new PageScreenshotOptions() { Path = "profile-changedphonenumber.png" });

		// check that the phone number textbox is now filled with the new number
		await Expect(Page.GetByPlaceholder("Enter your phone number")).ToHaveValueAsync(testPhoneNumber);

	}


}