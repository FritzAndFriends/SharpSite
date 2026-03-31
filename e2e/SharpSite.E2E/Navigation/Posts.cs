using Microsoft.Playwright;

namespace SharpSite.E2E.Navigation;

internal static class Posts
{
	public static async Task NavigateToPost(this IPage page, string postTitle)
	{
		// Navigate via admin post list — the home page can't list posts when
		// IPostRepository isn't registered through the PluginManager.
		await page.GotoAsync("/admin/posts");
		await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

		var link = page.GetByRole(AriaRole.Link, new() { Name = postTitle, Exact = true });
		var href = await link.GetAttributeAsync("href");

		// Convert admin URL (/admin/post/{date}/{slug}) to public URL (/{date}/{slug})
		var publicUrl = href!.Replace("/admin/post", "");
		await page.GotoAsync(publicUrl);
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

