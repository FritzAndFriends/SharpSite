using System.Text;

namespace SharpSite.Web;

public static class Program_RobotsTxt
{

	public static WebApplication MapRobotsTxt(this WebApplication app)
	{
		app.MapGet("/robots.txt", async (HttpContext context, ApplicationState appState) =>
		{

			context.Response.ContentType = "text/plain";

			var robotsTextContent = GenerateRobotsTxt($"i{context.Request.Scheme}://{context.Request.Host}", appState);

			await context.Response.WriteAsync(robotsTextContent);

		}).CacheOutput(policy =>
		{
			policy.Tag("robots");
			policy.Expire(TimeSpan.FromDays(30));
		});

		return app;
	}

	internal static string GenerateRobotsTxt(string urlBase, ApplicationState appState)
	{


		var sb = new StringBuilder();
		sb.AppendLine("User-agent: *");
		sb.AppendLine("Disallow: /admin/");

		if (!string.IsNullOrEmpty(appState.RobotsTxtCustomContent))
		{
			sb.AppendLine(appState.RobotsTxtCustomContent);
		}

		// add a line for the sitemap using the base URL from the context
		sb.AppendLine($"Sitemap: {urlBase}/sitemap.xml");

		return sb.ToString();


	}
}
