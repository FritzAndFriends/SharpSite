using Microsoft.Playwright;
using SharpSite.Abstractions;
using SharpSite.E2E.Abstractions;
using SharpSite.E2E.Navigation;

namespace SharpSite.E2E.Fixtures;

public class DeletePostTests : AuthenticatedPageTests
{
	// create a playwright test that logs in, navigates to the create post page, fills in the form and submits it
	[Fact]
	public async Task DeletePost()
	{

		// ARRANGE - create a post to delets
		const string PostTitle = "Test Post to delete";
		await LoginAsDefaultAdmin();

		await Page.NavigateToCreatePost();

		await Page.GetByPlaceholder("Title").ClickAsync();
		await Page.GetByPlaceholder("Title").FillAsync(PostTitle);

		await Page.GetByRole(AriaRole.Application).GetByRole(AriaRole.Textbox).FillAsync("This is a test");

		await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
		await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

		// ACT - now on the posts page, delete the post
		await Expect(Page.GetByRole(AriaRole.Cell, new() { Name = PostTitle, Exact = true })).ToBeVisibleAsync();
		await Page.GetByRole(AriaRole.Button, new() { Name = $"delete-{Post.GetSlug(PostTitle)}" }).ClickAsync();
		await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

		// ASSERT
		await Expect(Page.GetByRole(AriaRole.Cell, new() { Name = PostTitle, Exact = true })).Not.ToBeVisibleAsync();

	}


}