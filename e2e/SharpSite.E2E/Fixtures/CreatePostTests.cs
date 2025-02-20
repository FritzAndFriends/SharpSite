using Microsoft.Playwright;
using SharpSite.E2E.Abstractions;
using SharpSite.E2E.Navigation;

namespace SharpSite.E2E.Fixtures;


public class CreatePostTests : AuthenticatedPageTests
{

	// create a playwright test that logs in, navigates to the create post page, fills in the form and submits it
	[Fact]
	public async Task CreatePost()
	{
		const string PostTitle = "Test Post";

		await LoginAsDefaultAdmin();
		await Page.NavigateToCreatePost();

		await Page.GetByPlaceholder("Title").ClickAsync();
		await Page.GetByPlaceholder("Title").FillAsync(PostTitle);
		await Page.GetByRole(AriaRole.Application).GetByRole(AriaRole.Textbox).FillAsync("This is a test");

		await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
		await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);


		await Expect(Page.GetByRole(AriaRole.Cell, new() { Name = PostTitle, Exact = true })).ToBeVisibleAsync();

		await Page.NavigateToPost(PostTitle);

		var title = await Page.Locator("h1").InnerTextAsync();
		Assert.Equal(PostTitle, title);

	}

	// create a new post with a date in the past
	[Fact]
	public async Task CreatePostWithDateInPast()
	{
		const string PostTitle = "Test Post in the past";

		await LoginAsDefaultAdmin();
		await Page.NavigateToCreatePost();

		await Page.GetByPlaceholder("Title").ClickAsync();

		await Page.GetByPlaceholder("Title").FillAsync(PostTitle);
		await Page.GetByRole(AriaRole.Application).GetByRole(AriaRole.Textbox).FillAsync("This is a test");

		DateTime postDate = new DateTime(2020, 1, 1).Date;
		await Page.GetByLabel("Publish Date").FillAsync(postDate.ToString("yyyy-MM-dd"));
		await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
		await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

		await Expect(Page.GetByRole(AriaRole.Cell, new() { Name = PostTitle, Exact = true })).ToBeVisibleAsync();

		await Page.NavigateToPost(PostTitle);

		var title = await Page.Locator("h1").InnerTextAsync();
		Assert.Equal(PostTitle, title);

		// check that the date in the h6 is in the past
		var date = await Page.Locator("h6").InnerTextAsync();
		Assert.True(DateTime.TryParse(date, out var result));
		Assert.Equal(postDate, result.Date);


	}

}
