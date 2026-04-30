
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SharpSite.Abstractions;
using SharpSite.Data.Postgres;
using SharpSite.Security.Postgres;
using SharpSite.Web;
using SharpSite.Web.Components;
using SharpSite.Web.Locales;

var builder = WebApplication.CreateBuilder(args);

var appState = builder.AddPluginManagerAndAppState();

// Load plugins for postgres
#region Postgres Plugins
var pg = new RegisterPostgresServices();
await pg.AddServicesAtStartup(builder);

var pgSecurity = new RegisterPostgresSecurityServices();
pgSecurity.RegisterServices(builder);
#endregion

// add the custom localization features for the application framework
builder.ConfigureRequestLocalization();

builder.Services.AddHttpContextAccessor();

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
// Configure larger messages to allow upload of packages
builder.Services.Configure<HubOptions>(options =>
{
	options.EnableDetailedErrors = true;
});

// Add services to the container.
builder.Services.AddRazorComponents()
		.AddInteractiveServerComponents()
		.AddCircuitOptions(o =>
		{
			o.DetailedErrors = true;
		});


builder.Services.AddOutputCache();
// builder.Services.AddMemoryCache();

// add an implementation of IEmailSender that does nothing for SharpSiteUser
builder.Services.AddTransient<IEmailSender<SharpSiteUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

// StartupConfigMiddleware handles redirect-to-setup-wizard for non-started apps.
// The /startapi endpoint is mapped separately below to avoid Blazor's catch-all route.
app.UseMiddleware<StartupConfigMiddleware>();

app.UseHttpsRedirection();

app.ConfigurePluginFileSystem();

app.UseOutputCache();

// add error handlers for page not found
// TODO: UseStatusCodePagesWithReExecute causes 'RemoteNavigationManager already initialized'
// in .NET 10 Blazor SSR when a component sets a non-200 status code. Track in a separate issue.
// app.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");

app.UseAntiforgery();

// Redirect authenticated users who must change their default password
app.UseMiddleware<ForcePasswordChangeMiddleware>();

var pluginManager = await app.ActivatePluginManager(appState);

app.MapRazorComponents<App>()
		.AddInteractiveServerRenderMode()
		.AddAdditionalAssemblies(
		typeof(SharpSite.Security.Postgres.PgSharpSiteUser).Assembly
		//typeof(Sample.FirstThemePlugin.Theme).Assembly
		);

pgSecurity.MapEndpoints(app);


app.MapSiteMap();
app.MapRobotsTxt();
app.MapRssFeed();
app.MapDefaultEndpoints();
app.MapStartApi(appState);

app.UseRequestLocalization();

// Database initialization is triggered via /startapi (used by E2E tests)
// or through the startup wizard flow (Step3).

app.MapFileApi(pluginManager);

await app.RunAsync();
