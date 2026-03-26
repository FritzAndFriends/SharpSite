using SharpSite.Abstractions;
using SharpSite.Abstractions.DataStorage;
using SharpSite.Security.Postgres;
using SharpSite.Web;

public static class ProgramExtensions_StartApi
{
	public static WebApplication MapStartApi(this WebApplication app, ApplicationState appState)
	{
		app.MapPost("/startapi", async (HttpContext context, IConfiguration config, PluginManager pluginManager) =>
		{
			if (appState.StartupCompleted)
			{
				return Results.StatusCode(StatusCodes.Status202Accepted);
			}

			var state = await context.Request.ReadFromJsonAsync<ApplicationStateModel>();
			if (state is not null)
			{
				appState.MaximumUploadSizeMB = state.MaximumUploadSizeMB;
				appState.PageNotFoundContent = state.PageNotFoundContent;
				appState.RobotsTxtCustomContent = state.RobotsTxtCustomContent;
				appState.SiteName = state.SiteName;

				// Set connection strings from Aspire-injected configuration
				var connectionString = config.GetConnectionString("SharpSite") ?? string.Empty;
				appState.ContentConnectionString = connectionString;
				appState.SecurityConnectionString = connectionString;

				appState.StartupCompleted = true;
			}

			try
			{
				// Initialize content database via the data storage plugin
				var dataConfig = pluginManager.GetPluginProvidedService<IConfigureDataStorage>();
				if (dataConfig is not null)
				{
					await dataConfig.CreateNewDataStorage(appState);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Warning: Content DB init failed (may not have a data plugin loaded): {ex.Message}");
			}

			try
			{
				// Initialize security database (create schema, roles, and default admin user)
				var pgSecurity = new RegisterPostgresSecurityServices();
				await pgSecurity.ConfigureHttpApp(app);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Warning: Security DB init failed: {ex.Message}");
			}

			return Results.Ok();
		}).DisableAntiforgery();

		return app;
	}
}
