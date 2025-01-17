using System.Net;
using System.Security;
using System.Text;
using SharpSite.Abstractions;

namespace SharpSite.Web;

public static class Program_Rss
{

	public static WebApplication? MapRssFeed(this WebApplication? app)
	{

		if (app == null)
		{
			return null;
		}

		app.MapGet("/rss.xml", async (HttpContext context, IPostRepository postRepository) =>
		{

			var sb = new StringBuilder();
			sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
			sb.AppendLine("<rss version=\"2.0\">");
			sb.AppendLine("<channel>");
			sb.AppendLine("<title>SharpSite</title>");
			
			// generate the feed link from the current request using the scheme and host name
			sb.AppendLine($"<link>{context.Request.Scheme}://{context.Request.Host}</link>");

			var posts = await postRepository.GetPosts();

			foreach (var post in posts)
			{

				// generate the post link from the current request using the scheme and host name
				var postLink = $"{context.Request.Scheme}://{context.Request.Host}{post.ToUrl()}";
				// add the post to the stringbuilder xml document
				sb.AppendLine("<item>");
				sb.AppendLine($"<title>{SecurityElement.Escape(post.Title)}</title>");
				sb.AppendLine($"<link>{postLink}</link>");
				sb.AppendLine($"<description>{SecurityElement.Escape(post.Description)}</description>");
				sb.AppendLine($"<pubDate>{post.PublishedDate.ToString("R")}</pubDate>");
				sb.AppendLine("</item>");

			}

			sb.AppendLine("</channel>");
			sb.AppendLine("</rss>");

			context.Response.StatusCode = (int)HttpStatusCode.OK;
			context.Response.ContentType = "application/rss+xml";
			await context.Response.WriteAsync(sb.ToString());

			}).CacheOutput(policy =>
			{
				policy.Tag("rss");
				policy.Expire(TimeSpan.FromMinutes(30));
			});

		return app;

	}

}