using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace SharpSite.E2E;

public abstract class SharpSitePageTest : PageTest
{

	public override BrowserNewContextOptions ContextOptions()
	{
		return new BrowserNewContextOptions()
		{
			ColorScheme = ColorScheme.Light,
			Locale = "en-US",
			ViewportSize = new()
			{
				// set the viewport to 1024x768
				Width = 1024,
				Height = 768,
			},
			BaseURL = Environment.GetEnvironmentVariable("e2e-host") ?? "http://localhost:5020",
		};
	}


}