global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharpSite.Abstractions;
using SharpSite.Abstractions.Base;
using SharpSite.Abstractions.Security;
using System.Diagnostics;
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
		builder.Services.AddScoped<IEmailSender<ISharpSiteUser>, PgEmailSender>();

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
			activity?.AddEvent(new ActivityEvent("Created admin user with password 'Admin123!'"));
			await userManager.AddToRoleAsync(admin, Constants.Roles.Admin);
			activity?.AddEvent(new ActivityEvent("Assigned admin user to Admin role"));
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
