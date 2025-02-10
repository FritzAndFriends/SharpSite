using SharpSite.Abstractions;
using SharpSite.Web;
using System.Security;
using Xunit;

namespace SharpSite.Tests.Web.RSS;

public class GenerateRss
{
	[Fact]
	public void ReturnsValidRssFormat()
	{

		// Arrange
		var urlBase = "http://localhost";
		IEnumerable<Post> posts = [
			new Post
			{
				Slug = "test-post-1",
				Title = "Test Post 1",
				Description = "Description for Test Post 1",
				Content = "Content for Test Post 1",
				PublishedDate = DateTimeOffset.Now,
				LastUpdate = DateTimeOffset.Now,
				LanguageCode = "en"
			},
			new Post
			{
				Slug = "test-post-2",
				Title = "Test Post 2",
				Description = "Description for Test Post 2",
				Content = "Content for Test Post 2",
				PublishedDate = DateTimeOffset.Now,
				LastUpdate = DateTimeOffset.Now,
				LanguageCode = "en"
			}
		];

		// Act
		var rss = Program_Rss.GenerateRSS(urlBase, posts);

		// Assert
		Assert.Contains("<?xml version=\"1.0\" encoding=\"utf-8\"?>", rss);
		Assert.Contains("<rss version=\"2.0\">", rss);
		Assert.Contains("<channel>", rss);
		Assert.Contains("<title>SharpSite</title>", rss);
		Assert.Contains("<link>http://localhost</link>", rss);
		foreach (var post in posts)
		{
			Assert.Contains($"<title>{SecurityElement.Escape(post.Title)}</title>", rss);
			Assert.Contains($"<link>http://localhost{post.ToUrl()}</link>", rss);
			Assert.Contains($"<description>{SecurityElement.Escape(post.Description)}</description>", rss);
			Assert.Contains($"<pubDate>{post.PublishedDate:R}</pubDate>", rss);
		}
		Assert.Contains("</channel>", rss);
		Assert.Contains("</rss>", rss);

	}
}

