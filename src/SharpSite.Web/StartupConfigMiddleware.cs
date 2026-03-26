using SharpSite.Abstractions;
using SharpSite.Web;

public class StartupConfigMiddleware(RequestDelegate next, ApplicationState AppState)
{

	public async Task Invoke(HttpContext context)
	{

		// Check if the application is started and skip the middleware if it is.
		if (AppState.StartupCompleted)
		{
			await next(context);
			return;
		}

		// Redirect to the start page if the application is not started yet.
		if (context.Request.Path.Value is not null &&
			!context.Request.Path.Value.StartsWith("/start") &&
			!context.Request.Path.Value.StartsWith("/_blazor") &&
			!context.Request.Path.Value.EndsWith(".js") &&
			!context.Request.Path.Value.EndsWith(".css") &&
			!context.Request.Path.Value.Contains("/img/"))
		{
			Console.WriteLine("Redirecting for first start");
			context.Response.Redirect("/start/step1");
		}

		await next(context);

	}

}
