using Microsoft.Playwright;

namespace SharpSite.E2E;

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
