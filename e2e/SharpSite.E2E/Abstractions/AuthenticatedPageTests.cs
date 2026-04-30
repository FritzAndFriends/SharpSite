using Microsoft.Playwright;

namespace SharpSite.E2E.Abstractions;

/// <summary>
/// This class is used to test pages where we are logged in as a user.
/// </summary>
[WithTestName]
public abstract class AuthenticatedPageTests : SharpSitePageTest
{
	private const string URL_LOGIN = "/Account/Login";
	private const string LOGIN_USERID = "admin@Localhost";
	private const string LOGIN_PASSWORD = "Admin123!";
	private const string NEW_PASSWORD = "Admin456!";

	// Tracks whether the default admin password has been changed via ForceChangePassword.
	// Safe because all tests in the [Collection] run sequentially.
	private static bool _passwordChanged = false;

	private static string CurrentPassword => _passwordChanged ? NEW_PASSWORD : LOGIN_PASSWORD;

	public static readonly bool RunTrace = true;

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		Context.SetDefaultNavigationTimeout(30000);
		Context.SetDefaultTimeout(30000);

		if (RunTrace)
		{
			await Context.Tracing.StartAsync(new()
			{
				Title = $"{WithTestNameAttribute.CurrentClassName}.{WithTestNameAttribute.CurrentTestName}",
				Screenshots = true,
				Snapshots = true,
				Sources = true
			});
		}

	}

	public override async Task DisposeAsync()
	{

		if (RunTrace)
			await Context.Tracing.StopAsync(new()
			{
				Path = Path.Combine(
						Environment.CurrentDirectory,
						"playwright-traces",
					 $"{WithTestNameAttribute.CurrentClassName}.{WithTestNameAttribute.CurrentTestName}.zip"
				)
			});
		await base.DisposeAsync().ConfigureAwait(false);
	}


	protected async Task LoginAsDefaultAdmin()
	{

		await Page.GotoAsync(URL_LOGIN);
		await Page.GetByRole(AriaRole.Textbox, new() { Name = "Input.Email" })
			.FillAsync(LOGIN_USERID);
		await Page.GetByRole(AriaRole.Textbox, new() { Name = "Input.Password" })
			.FillAsync(CurrentPassword);
		await Page.GetByRole(AriaRole.Button, new() { Name = "loginbutton" }).ClickAsync();
		await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

		// Handle forced password change if the admin account was just seeded
		if (Page.Url.Contains("/Account/ForceChangePassword"))
		{
			await Page.Locator("#Input\\.CurrentPassword").FillAsync(LOGIN_PASSWORD);
			await Page.Locator("#Input\\.NewPassword").FillAsync(NEW_PASSWORD);
			await Page.Locator("#Input\\.ConfirmPassword").FillAsync(NEW_PASSWORD);
			await Page.GetByRole(AriaRole.Button, new() { Name = "Change password" }).ClickAsync();
			await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
			_passwordChanged = true;
		}

	}

	protected async Task Logout()
	{
		await Page.GetByRole(AriaRole.Button, new() { Name = "Logout" }).ClickAsync();
	}

}


