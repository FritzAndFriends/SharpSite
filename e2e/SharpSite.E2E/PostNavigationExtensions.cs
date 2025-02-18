using Microsoft.Playwright;

namespace SharpSite.E2E;

internal static class PostNavigationExtensions
{
	public static async Task NavigateToPost(this IPage page, string postTitle)
	{
		await page.GotoAsync("/");
		await page.GetByRole(AriaRole.Link, new() { Name = postTitle, Exact = true }).ClickAsync();
		await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
		await Task.Delay(1000);
	}

	// navigate to the create post page
	public static async Task NavigateToCreatePost(this IPage page)
	{
		await page.GotoAsync("/admin/post");
		await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
	}

}

