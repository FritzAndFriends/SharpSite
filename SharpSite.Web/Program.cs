using Microsoft.AspNetCore.SignalR;
using SharpSite.Data.Postgres;
using SharpSite.Security.Postgres;
using SharpSite.Web;
using SharpSite.Web.Components;
using SharpSite.Web.Locales;

var builder = WebApplication.CreateBuilder(args);

// Load plugins for postgres
#region Postgres Plugins
var pg = new RegisterPostgresServices();
pg.RegisterServices(builder);

var pgSecurity = new RegisterPostgresSecurityServices();
pgSecurity.RegisterServices(builder);
#endregion

var appState = builder.AddPluginManagerAndAppState();

// add the custom localization features for the application framework
builder.ConfigureRequestLocalization();

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
builder.Services.AddMemoryCache();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.ConfigurePluginFileSystem();

app.UseAntiforgery();

app.UseOutputCache();

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

app.UseRequestLocalization();

await pgSecurity.RunAtStartup(app.Services);

app.MapFileApi(pluginManager);

app.Run();
