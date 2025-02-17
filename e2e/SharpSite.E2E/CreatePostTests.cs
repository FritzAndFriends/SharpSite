using Microsoft.Playwright;

namespace SharpSite.E2E;

public class CreatePostTests : AuthenticatedPageTests
{

	// create a playwright test that logs in, navigates to the create post page, fills in the form and submits it
	[Fact]
	public async Task CreatePost()
	{
		await LoginAsDefaultAdmin();
		await Page.GotoAsync("/admin/post");
		await Page.GetByPlaceholder("Title").ClickAsync();
		await Page.GetByPlaceholder("Title").FillAsync("Test Post");
		await Page.GetByRole(AriaRole.Application).GetByRole(AriaRole.Textbox).FillAsync("This is a test");

		await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

		await Page.ScreenshotAsync(new()
		{
			Path = "testpost-postlist.png",
			FullPage = true,
		});

		await Expect(Page.GetByRole(AriaRole.Cell, new() { Name = "Test post" })).ToBeVisibleAsync();

		await Page.GotoAsync("/");
		await Page.ScreenshotAsync(new()
		{
			Path = "testpost-home.png",
			FullPage = true,
		});

		await Page.GetByRole(AriaRole.Link, new() { Name = "Test Post" }).ClickAsync();

		await Page.ScreenshotAsync(new()
		{
			Path = "testpost.png",
			FullPage = true,
		});


		var title = await Page.Locator("h1").InnerTextAsync();
		Assert.Equal("Test Post", title);
		await Logout();
	}


}
