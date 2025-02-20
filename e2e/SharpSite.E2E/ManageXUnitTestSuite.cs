using Microsoft.Playwright;

namespace SharpSite.E2E;

public class ManageXUnitTestSuite //: IAsyncLifetime
{


	private const string URL_LOGIN = "/Account/Login";
	private const string LOGIN_USERID = "admin@Localhost";
	private const string LOGIN_PASSWORD = "Admin123!";

	public Task DisposeAsync()
	{
		if (File.Exists(".auth.json")) File.Delete(".auth.json");

		return Task.CompletedTask;
	}

	public async Task InitializeAsync()
	{
		if (File.Exists(".auth.json")) File.Delete(".auth.json");

		using var playwright = await Playwright.CreateAsync();
		await using var browser = await playwright.Chromium.LaunchAsync();
		var context = await browser.NewContextAsync(new BrowserNewContextOptions()
		{
			ColorScheme = ColorScheme.Light,
			Locale = "en-US",
			ViewportSize = new()
			{
				// set the viewport to 1024x768
				Width = 1024,
				Height = 768,
			},
			BaseURL = "http://localhost:5020"
		});
		// create a new page
		var page = await context.NewPageAsync();
		await page.GotoAsync(URL_LOGIN);
		await page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
		await page.GetByRole(AriaRole.Textbox, new() { Name = "Input.Email" })
			.FillAsync(LOGIN_USERID);
		await page.GetByRole(AriaRole.Textbox, new() { Name = "Input.Password" })
			.FillAsync(LOGIN_PASSWORD);
		await page.GetByRole(AriaRole.Button, new() { Name = "loginbutton" }).ClickAsync();
		await context.StorageStateAsync(new()
		{
			Path = ".auth.json"
		});

	}
}


