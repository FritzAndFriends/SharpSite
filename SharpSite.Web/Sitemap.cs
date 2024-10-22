using System.Text;
using SharpSite.Abstractions;

public static class ProgramExtensions_Sitemap
{
	public static WebApplication MapSiteMap(this WebApplication app)
	{
		app.MapGet("/sitemap.xml", async (
			IHostEnvironment env, 
			HttpContext context, 
			IPostRepository postRepository) =>
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
						<lastmod>{post.PublishedDate.ToString("yyyy-MM-dd")}</lastmod>
					</url>
				""";
				sb.Append(newXml);
			}

			// append post URLs

			sb.Append("</urlset>");

			context.Response.ContentType = "application/xml";
			await context.Response.WriteAsync(sb.ToString());
		});
		return app;

	}

}
	
