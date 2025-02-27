﻿using SharpSite.Abstractions;
using SharpSite.Web;
using System.Text.Json;

public class StartupConfigMiddleware(RequestDelegate next, ApplicationState AppState)
{

	public async Task Invoke(HttpContext context)
	{

		// Exit now if the app is already configured
		if (!AppState.StartupCompleted &&
			!context.Request.Path.Value!.StartsWith("/start") &&
			!context.Request.Path.Value.EndsWith(".js") &&
			!context.Request.Path.Value.EndsWith(".css") &&
			!context.Request.Path.Value.Contains("/img/"))
		{
			Console.WriteLine("Redirecting for first start");
			context.Response.Redirect("/start/step1");
		}
		else if (context.Request.Path.Value!.StartsWith("/startapi") && context.Request.Method == "POST")
		{

			if (AppState.StartupCompleted)
			{
				context.Response.StatusCode = StatusCodes.Status202Accepted;
				return;
			}

			var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
			var state = JsonSerializer.Deserialize<ApplicationStateModel>(body, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			});
			if (state is not null)
			{
				//AppState.ConfigurationSections = state.ConfigurationSections;
				//AppState.CurrentTheme = state.CurrentTheme;
				AppState.MaximumUploadSizeMB = state.MaximumUploadSizeMB;
				//AppState.Localization = state.Localization;
				AppState.PageNotFoundContent = state.PageNotFoundContent;
				AppState.RobotsTxtCustomContent = state.RobotsTxtCustomContent;
				AppState.SiteName = state.SiteName;
				AppState.StartupCompleted = true;
				//await AppState.Save();

			}

			context.Response.StatusCode = StatusCodes.Status200OK;
			return;

		}

		await next(context);

	}

}
