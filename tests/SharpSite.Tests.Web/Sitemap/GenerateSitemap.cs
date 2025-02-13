using SharpSite.Abstractions;
using System.Text.RegularExpressions;
using Xunit;

namespace SharpSite.Tests.Web.Sitemap;
public partial class GenerateSitemap
{
	private const string _Host = "example.com";

	[Fact]
	public void ShouldReturnValidXml()
	{
		// Arrange
		var now = DateTimeOffset.Now;
		IEnumerable<Post> posts = [
			new Post { PublishedDate = now, Slug = "post-1", Title = "Post 1", Content = "Content 1" },
			new Post { PublishedDate = now, Slug = "post-2", Title = "Post 2", Content = "Content 2" }
		];

		IEnumerable<Page> pages = [
			new Page { LastUpdate = now, Slug = "page-1" },
			new Page { LastUpdate = now, Slug = "page-2" }
		];

		// Act
		var result = ProgramExtensions_Sitemap.GenerateSitemap(_Host, posts, pages);

		// Assert
		Assert.NotNull(result);
		Assert.Contains("<loc>https://example.com</loc>", result);
		Assert.Contains($"https://example.com/{now.UtcDateTime:yyyyMMdd}/post-1", result);
		Assert.Contains($"https://example.com/{now.UtcDateTime:yyyyMMdd}/post-2", result);
		Assert.Contains("https://example.com/page-1", result);
		Assert.Contains("https://example.com/page-2", result);
	}

	[Fact]
	public void ShouldHandleEmptyRepositories()
	{
		// Act
		var result = ProgramExtensions_Sitemap.GenerateSitemap(_Host, [], []);

		// Assert
		Assert.NotNull(result);
		Assert.Contains("<loc>https://example.com</loc>", result);
		Assert.DoesNotMatch(PostUrlRegex(), result);
		Assert.DoesNotContain("https://example.com/page-", result);
	}

	[GeneratedRegex(@"https://example.com/\d{8}/post-")]
	private static partial Regex PostUrlRegex();
}
