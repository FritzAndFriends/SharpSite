using System.Text;

namespace SharpSite.Web;

public static class Program_RobotsTxt
{

	public static WebApplication MapRobotsTxt(this WebApplication app)
	{
		app.MapGet("/robots.txt", async (HttpContext context) =>
		{
			context.Response.ContentType = "text/plain";

			var sb = new StringBuilder();
			sb.AppendLine("User-agent: *");
			sb.AppendLine("Disallow: /admin/");

			// add a line for the sitemap using the base URL from the context
			sb.AppendLine($"Sitemap: {context.Request.Scheme}://{context.Request.Host}/sitemap.xml");

			await context.Response.WriteAsync(sb.ToString());
		}).CacheOutput(policy =>
		{
			policy.Tag("robots");
			policy.Expire(TimeSpan.FromDays(30));
		});


		return app;

	}

}