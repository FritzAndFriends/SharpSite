using Microsoft.Playwright;
using SharpSite.Abstractions;
using System.Net.Http.Json;

namespace SharpSite.E2E;

[CollectionDefinition(TEST_COLLECTION_NAME)]
public class WebsiteConfigurationFixtureCollection : ICollectionFixture<WebsiteConfigurationFixture>
{
	public const string TEST_COLLECTION_NAME = "Website collection";
	// This class has no code, and is never created. Its purpose is simply
	// to be the place to apply [CollectionDefinition] and all the
	// ICollectionFixture<> interfaces.
}


public class WebsiteConfigurationFixture
{


	private const string URL_LOGIN = "/Account/Login";
	private const string LOGIN_USERID = "admin@Localhost";
	private const string LOGIN_PASSWORD = "Admin123!";


	public WebsiteConfigurationFixture()
	{

		//using var playwright = await Playwright.CreateAsync();
		//await using var browser = await playwright.Chromium.LaunchAsync();
		//var context = await browser.NewContextAsync(new BrowserNewContextOptions()
		//{
		//	ColorScheme = ColorScheme.Light,
		//	Locale = "en-US",
		//	ViewportSize = new()
		//	{
		//		// set the viewport to 1024x768
		//		Width = 1024,
		//		Height = 768,
		//	},
		//	BaseURL = "http://localhost:5020"
		//});

		//await CreateAuthTicket(context);

		ConfigureSharpsiteAsExistingWebsite().GetAwaiter().GetResult();

	}

	private async Task ConfigureSharpsiteAsExistingWebsite()
	{

		// create an applicationState object and POST it to ./startapi
		var appState = new ApplicationStateModel()
		{
			SiteName = "My Playwright Test Site",
			MaximumUploadSizeMB = 10,
			//CurrentTheme = "SharpSite.Web.DefaultTheme",
			RobotsTxtCustomContent = "User-agent: *\nDisallow: /",
			PageNotFoundContent = "<h1>Page not found</h1>",
			StartupCompleted = true,

		};

		// post AppState to the /startapi endpoint using an http client
		var client = new HttpClient();
		client.BaseAddress = new Uri("http://localhost:5020");
		var response = await client.PostAsJsonAsync("/startapi", appState);
		response.EnsureSuccessStatusCode();


	}

	private static async Task CreateAuthTicket(IBrowserContext context)
	{
		if (File.Exists(".auth.json")) File.Delete(".auth.json");
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

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}


