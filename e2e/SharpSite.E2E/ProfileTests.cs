using Microsoft.Playwright;

namespace SharpSite.E2E;

public class ProfileTests : AuthenticatedPageTests
{

	[Fact]
	public async Task CanViewProfile()
	{

		await LoginAsDefaultAdmin();

		await Page.GotoAsync("/Account/Manage");
		await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

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
		await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

		await Page.GetByPlaceholder("Enter your phone number").ClickAsync();
		await Page.GetByPlaceholder("Enter your phone number").FillAsync(testPhoneNumber);
		await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
		await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

		await Page.ScreenshotAsync(new PageScreenshotOptions() { Path = "profile-changedphonenumber.png" });

		// check that the phone number textbox is now filled with the new number
		await Expect(Page.GetByPlaceholder("Enter your phone number")).ToHaveValueAsync(testPhoneNumber);

	}


}