using SharpSite.Abstractions;
using System.Text;

public static class ProgramExtensions_Sitemap
{
	public static WebApplication MapSiteMap(this WebApplication app)
	{
		app.MapGet("/sitemap.xml", async (
			IHostEnvironment env,
			HttpContext context,
			IPostRepository postRepository,
			IPageRepository pageRepository) =>
		{

			var host = context.Request.Host.Value;
			var lastModDate = DateTime.Now.Date;

			var baseXML = $"""
				<?xml version="1.0" encoding="UTF-8"?>
				<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
				  <url>
				    <loc>https://{host}</loc>
				    <lastmod>{lastModDate.ToString("yyyy-MM-dd")}</lastmod>
				  </url>
				""";

			var sb = new StringBuilder(baseXML);

			var posts = await postRepository.GetPosts();

			foreach (var post in posts)
			{
				var newXml = $"""
					<url>
						<loc>https://{host}{post.ToUrl()}</loc>
						<lastmod>{post.LastUpdate.ToString("yyyy-MM-dd")}</lastmod>
					</url>
				""";
				sb.Append(newXml);
			}

			// append page URLs
			var pages = await pageRepository.GetPages();
			foreach (var page in pages)
			{
				var newXml = $"""
					<url>
						<loc>https://{host}/{page.Slug.ToLowerInvariant()}</loc>
						<lastmod>{page.LastUpdate.ToString("yyyy-MM-dd")}</lastmod>
					</url>
				""";
				sb.Append(newXml);
			}

			sb.Append("</urlset>");

			context.Response.ContentType = "application/xml";
			await context.Response.WriteAsync(sb.ToString());
		})
		.CacheOutput(policy =>
		{
			policy.Tag("sitemap");
			policy.Expire(TimeSpan.FromMinutes(30));
		});
		return app;

	}

}
