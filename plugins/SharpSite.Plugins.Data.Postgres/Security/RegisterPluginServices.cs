global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.UI.Services;
global using Microsoft.Extensions.Logging;
global using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharpSite.Abstractions;
using SharpSite.Abstractions.Base;
using SharpSite.Abstractions.Security;

namespace SharpSite.Plugins.Data.Postgres.Security;

// TODO: Remove this and move the Identity configuration to the main project,
// the database context and Identity providers will be injected from the PluginManager


public class RegisterPluginServices : IRunAtStartup
{
    private const string InitializeUsersActivitySourceName = "Initial Users and Roles";

    public Task RunOnUninstall()
    {
        return Task.CompletedTask;
    }

    public async Task<IHostApplicationBuilder> AddServicesAtStartup(IHostApplicationBuilder builder)
    {
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider<PgSharpSiteUser>>();

        // Register our repositories and services
        builder.Services.AddScoped<IUserManager<ISharpSiteUser>, PgUserManager>();
        builder.Services.AddScoped<ISignInManager<ISharpSiteUser>, PgSignInManager>();

        // Configure email senders
        builder.Services.AddScoped<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, NoOpEmailSender>();
        builder.Services.AddScoped<SharpSite.Abstractions.Security.IEmailSender<ISharpSiteUser>, PgEmailSender>();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
        .AddIdentityCookies();

        builder.Services.AddIdentityCore<PgSharpSiteUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<PgContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing => tracing.AddSource(InitializeUsersActivitySourceName));

        return builder;
    }

    public async Task<IApplicationBuilder> ConfigureHttpApp(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var provider = scope.ServiceProvider;
        var dbContext = provider.GetRequiredService<PgContext>();
        await dbContext.Database.MigrateAsync();

        var roleMgr = provider.GetRequiredService<RoleManager<IdentityRole>>();

        // Create default roles
        if (!await roleMgr.RoleExistsAsync(Constants.Roles.Admin))
        {
            await roleMgr.CreateAsync(new IdentityRole(Constants.Roles.Admin));
        }

        if (!await roleMgr.RoleExistsAsync(Constants.Roles.Editor))
        {
            await roleMgr.CreateAsync(new IdentityRole(Constants.Roles.Editor));
        }

        if (!await roleMgr.RoleExistsAsync(Constants.Roles.User))
        {
            await roleMgr.CreateAsync(new IdentityRole(Constants.Roles.User));
        }

        var userManager = provider.GetRequiredService<UserManager<PgSharpSiteUser>>();
        if (!await userManager.Users.AnyAsync())
        {
            var admin = new PgSharpSiteUser
            {
                DisplayName = "Admin",
                UserName = "admin@localhost",
                Email = "admin@localhost",
                EmailConfirmed = true
            };
            var newUserResult = await userManager.CreateAsync(admin, "Admin123!");
            if (newUserResult.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, Constants.Roles.Admin);
            }
        }

        return app;
    }

    public Task RunAtStartup()
    {
        return Task.CompletedTask;
    }

    public Task RunOnInstall()
    {
        return Task.CompletedTask;
    }

    public Task RunOnUpdate()
    {
        return Task.CompletedTask;
    }
}

internal sealed class NoOpEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // For development, just output to console
        Console.WriteLine($"Email: {email}, Subject: {subject}, Message: {htmlMessage}");
        return Task.CompletedTask;
    }
}

internal sealed class IdentityUserAccessor(IUserManager userManager, IdentityRedirectManager redirectManager)
{
    public async Task<ISharpSiteUser> GetRequiredUserAsync(HttpContext context)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null)
        {
            redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
        }

        return user!;
    }
}

internal sealed class IdentityRedirectManager(NavigationManager navigationManager)
{
    public void RedirectTo(string uri)
    {
        navigationManager.NavigateTo(uri);
    }

    public void RedirectTo(string uri, Dictionary<string, object?> queryParameters)
    {
        navigationManager.NavigateTo(navigationManager.GetUriWithQueryParameters(uri, queryParameters));
    }

    public void RedirectToWithStatus(string uri, string message, HttpContext context)
    {
        context.Response.Cookies.Append("StatusMessage", message);
        navigationManager.NavigateTo(uri);
    }
}

internal sealed class IdentityRevalidatingAuthenticationStateProvider<TUser> : AuthenticationStateProvider where TUser : class
{
    private readonly IPluginManager _pluginManager;
    private readonly ILogger<IdentityRevalidatingAuthenticationStateProvider<TUser>> _logger;

	// need to modify the constructor to accept the PluginManager
	// and use it to resolve the UserManager and SignInManager
	public IdentityRevalidatingAuthenticationStateProvider(
			IPluginManager pluginManager,
			ILogger<IdentityRevalidatingAuthenticationStateProvider<TUser>> logger)
	{
		_pluginManager = pluginManager;
		_logger = logger;
	}

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        try
        {
            var userManager = _pluginManager.GetPluginProvidedService<UserManager<TUser>>();
            var signInManager = _pluginManager.GetPluginProvidedService<SignInManager<TUser>>();

						if (userManager is null || signInManager is null)
						{
								_logger.LogWarning("UserManager or SignInManager not found in plugin services.");
								return new AuthenticationState(principal);
						}

            var user = await signInManager.Context.AuthenticateAsync(IdentityConstants.ApplicationScheme);
            if (user?.Principal is null)
            {
                return new AuthenticationState(principal);
            }

            return new AuthenticationState(user.Principal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAuthenticationStateAsync");
            return new AuthenticationState(principal);
        }
    }
}
