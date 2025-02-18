using Microsoft.Playwright;

namespace SharpSite.E2E;

/// <summary>
/// This class is used to test pages where we are logged in as a user.
/// </summary>
[WithTestName]
public abstract class AuthenticatedPageTests : SharpSitePageTest
{
	private const string URL_LOGIN = "/Account/Login";
	private const string LOGIN_USERID = "admin@Localhost";
	private const string LOGIN_PASSWORD = "Admin123!";

	public static readonly bool RunTrace = true;

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		Context.SetDefaultNavigationTimeout(10000);
		Context.SetDefaultTimeout(10000);

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
		//await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
		await Page.GetByRole(AriaRole.Textbox, new() { Name = "Input.Email" })
			.FillAsync(LOGIN_USERID);
		await Page.GetByRole(AriaRole.Textbox, new() { Name = "Input.Password" })
			.FillAsync(LOGIN_PASSWORD);
		await Page.GetByRole(AriaRole.Button, new() { Name = "loginbutton" }).ClickAsync();
		//await Context.StorageStateAsync(new()
		//{
		//	Path = ".auth.json"
		//});
		await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
		Console.WriteLine("Logged in as " + LOGIN_USERID);

	}

	protected async Task Logout()
	{
		await Page.GetByRole(AriaRole.Button, new() { Name = "Logout" }).ClickAsync();
	}

}


