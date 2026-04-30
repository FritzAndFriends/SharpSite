using SharpSite.Abstractions;
using SharpSite.Abstractions.DataStorage;
using SharpSite.Data.Postgres;
using SharpSite.Security.Postgres;
using SharpSite.Web;

public static class ProgramExtensions_StartApi
{
	public static WebApplication MapStartApi(this WebApplication app, ApplicationState appState)
	{
		app.MapPost("/startapi", async (HttpContext context, IConfiguration config) =>
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

			using var scope = app.Services.CreateScope();

			try
			{
				// Initialize content database schema
				Console.WriteLine("StartApi: Initializing content database...");
				var pgContext = scope.ServiceProvider.GetRequiredService<PgContext>();
				await pgContext.Database.EnsureCreatedAsync();
				Console.WriteLine("StartApi: Content database initialized successfully");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"ERROR: Content DB init failed: {ex}");
			}

			try
			{
				// Initialize security database (create schema, roles, and default admin user)
				Console.WriteLine("StartApi: Initializing security database...");
				var pgSecurity = new RegisterPostgresSecurityServices();
				await pgSecurity.ConfigureHttpApp(app);
				Console.WriteLine("StartApi: Security database initialized successfully");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"ERROR: Security DB init failed: {ex}");
			}

			return Results.Ok();
		}).DisableAntiforgery();

		return app;
	}
}
