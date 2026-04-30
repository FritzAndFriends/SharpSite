global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharpSite.Abstractions;
using SharpSite.Abstractions.Base;
using AbsSecurity = SharpSite.Abstractions.Security;
using System.Diagnostics;
using System.Security.Claims;
using Constants = SharpSite.Abstractions.Constants;

namespace SharpSite.Security.Postgres;

public class RegisterPostgresSecurityServices : IRunAtStartup
{
	private const string InitializeUsersActivitySourceName = "Initial Users and Roles";

	public IHostApplicationBuilder RegisterServices(IHostApplicationBuilder builder, bool disableRetry = false)
	{

		builder.Services.AddCascadingAuthenticationState();
		builder.Services.AddScoped<IdentityUserAccessor>();
		builder.Services.AddScoped<IdentityRedirectManager>();
		builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

		// Register our repositories and services
		builder.Services.AddScoped<IUserRepository, UserRepository>();
		builder.Services.AddScoped<AbsSecurity.IEmailSender, PgEmailSender>();

		builder.Services.AddAuthentication(options =>
		{
			options.DefaultScheme = IdentityConstants.ApplicationScheme;
			options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
		})
		.AddIdentityCookies();

		ConfigurePostgresDbContext(builder, disableRetry);
		builder.Services.AddIdentityCore<PgSharpSiteUser>(options => options.SignIn.RequireConfirmedAccount = true)
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<PgSecurityContext>()
				.AddSignInManager()
				.AddDefaultTokenProviders();

		builder.Services.AddOpenTelemetry()
			.WithTracing(tracing => tracing.AddSource(InitializeUsersActivitySourceName));


		builder.Services.AddSingleton<IEmailSender<PgSharpSiteUser>, IdentityNoOpEmailSender>();

		// Register the non-generic MS IEmailSender needed by PgEmailSender
		builder.Services.AddSingleton<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender>(
			_ => new InternalNoOpEmailSender());

		return builder;

	}

	public static void ConfigurePostgresDbContext(IHostApplicationBuilder builder, bool disableRetry)
	{
		builder.AddNpgsqlDbContext<PgSecurityContext>(Data.Postgres.Constants.DBNAME, configure =>
		{
			configure.DisableRetry = disableRetry;
		}, configure =>
		{
			configure.UseNpgsql(options =>
			{
				options.MigrationsHistoryTable("__EFMigrationsHistory_Security");
			});
		});
	}

	public async Task<IApplicationBuilder> ConfigureHttpApp(IApplicationBuilder app)

	//public async Task RunAtStartup(IServiceProvider services)
	{

		var services = app.ApplicationServices;

		ActivitySource activitySource = new ActivitySource(InitializeUsersActivitySourceName);
		var activity = activitySource.CreateActivity("Inspecting roles", ActivityKind.Internal);

		using var scope = services.CreateScope();
		var provider = scope.ServiceProvider;

		// Create the Identity tables. We cannot use EnsureCreatedAsync() because the
		// content context (PgContext) already created the database and EnsureCreated
		// short-circuits when the database already has tables.
		var dbContext = provider.GetRequiredService<PgSecurityContext>();
		var creator = dbContext.Database.GetService<Microsoft.EntityFrameworkCore.Storage.IRelationalDatabaseCreator>();
		try
		{
			await creator.CreateTablesAsync();
		}
		catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P07")
		{
			// 42P07 = "relation already exists" — tables were created by a prior run
		}

		activity?.Start();
		var roleMgr = provider.GetRequiredService<RoleManager<IdentityRole>>();
		var adminExists = await roleMgr.RoleExistsAsync(Constants.Roles.Admin);
		if (!adminExists)
		{
			await roleMgr.CreateAsync(new IdentityRole(Constants.Roles.Admin));
			activity?.AddEvent(new ActivityEvent("Created Admin role"));
		}

		var editorExists = await roleMgr.RoleExistsAsync(Constants.Roles.Editor);
		if (!editorExists)
		{
			await roleMgr.CreateAsync(new IdentityRole(Constants.Roles.Editor));
			activity?.AddEvent(new ActivityEvent("Created Editor role"));
		}

		var userExists = await roleMgr.RoleExistsAsync(Constants.Roles.User);
		if (!userExists)
		{
			await roleMgr.CreateAsync(new IdentityRole(Constants.Roles.User));
			activity?.AddEvent(new ActivityEvent("Created User role"));
		}

		activity?.Stop();

		activity = activitySource.CreateActivity("Inspecting users", ActivityKind.Internal);
		activity?.Start();

		var userManager = provider.GetRequiredService<UserManager<PgSharpSiteUser>>();
		var anyUsers = await userManager.Users.AnyAsync();
		if (!anyUsers)
		{
			var admin = new PgSharpSiteUser
			{
				DisplayName = "Admin",
				UserName = "admin@localhost",
				Email = "admin@localhost",
				EmailConfirmed = true
			};
			var newUserResult = await userManager.CreateAsync(admin, "Admin123!");
			activity?.AddEvent(new ActivityEvent("Created admin user with default credentials"));
			await userManager.AddToRoleAsync(admin, Constants.Roles.Admin);
			activity?.AddEvent(new ActivityEvent("Assigned admin user to Admin role"));

			// Flag the admin user to force a password change on first login
			await userManager.AddClaimAsync(admin, new Claim("MustChangePassword", "true"));
			activity?.AddEvent(new ActivityEvent("Set forced password change flag for admin user"));
		}

		// In production, warn if the default admin password is still active
		var env = services.GetRequiredService<IHostEnvironment>();
		if (!env.IsDevelopment())
		{
			var adminUser = await userManager.FindByEmailAsync("admin@localhost");
			if (adminUser is not null && await userManager.CheckPasswordAsync(adminUser, "Admin123!"))
			{
				var logger = services.GetRequiredService<ILoggerFactory>()
					.CreateLogger<RegisterPostgresSecurityServices>();
				logger.LogWarning(
					"SECURITY WARNING: The default admin account (admin@localhost) still uses the initial seed password. " +
					"Change it immediately in a production environment!");
			}
		}

		return app;

	}

	public void CreateDatabaseIfNotExists(string connectionString)
	{

		// create the PgSecurityContext if it does not exist using the entity framework context with the connection string passed in
		var optionsBuilder = new DbContextOptionsBuilder<PgSecurityContext>();
		optionsBuilder.UseNpgsql<PgSecurityContext>(connectionString);
		using var context = new PgSecurityContext(optionsBuilder.Options);
		context.Database.EnsureCreated();

	}

	/// <summary>
	/// Updates the database schema to the latest versions
	/// </summary>
	/// <returns></returns>
	public Task UpdateDatabaseSchemaAsync(string connectionString)
	{

		// create the PgSecurityContext if it does not exist using the entity framework context with the connection string passed in
		var optionsBuilder = new DbContextOptionsBuilder<PgSecurityContext>();
		optionsBuilder.UseNpgsql<PgSecurityContext>(connectionString);
		using var context = new PgSecurityContext(optionsBuilder.Options);
		return context.Database.MigrateAsync();

	}

	public void MapEndpoints(IEndpointRouteBuilder endpointDooHickey)
	{
		endpointDooHickey.MapAdditionalIdentityEndpoints();
	}

	public Task RunOnInstall()
	{
		throw new NotImplementedException();
	}

	public Task RunOnUpdate()
	{
		throw new NotImplementedException();
	}

	public Task RunOnUninstall()
	{
		throw new NotImplementedException();
	}

	public Task<IHostApplicationBuilder> AddServicesAtStartup(IHostApplicationBuilder app)
	{
		return Task.FromResult(app);
	}
}

internal sealed class InternalNoOpEmailSender : Microsoft.AspNetCore.Identity.UI.Services.IEmailSender
{
	public Task SendEmailAsync(string email, string subject, string htmlMessage) => Task.CompletedTask;
}
