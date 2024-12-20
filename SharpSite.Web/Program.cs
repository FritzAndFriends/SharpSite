using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.FileProviders;
using SharpSite.Data.Postgres;
using SharpSite.Security.Postgres;
using SharpSite.Web;
using SharpSite.Web.Components;
using SharpSite.Web.Locales;
using SharpSite.Plugins;

var builder = WebApplication.CreateBuilder(args);

PluginManager.Initialize();

// Load plugins for postgres
#region Postgres Plugins
var pg = new RegisterPostgresServices();
pg.RegisterServices(builder);

var pgSecurity = new RegisterPostgresSecurityServices();
pgSecurity.RegisterServices(builder);
#endregion

// Configure applicatin state and the PluginManager
var appState = new ApplicationState();
await appState.Load();
builder.Services.AddSingleton(appState);
builder.Services.AddSingleton<PluginAssemblyManager>();
builder.Services.AddSingleton<PluginManager>();

// add the custom localization features for the application framework
builder.ConfigureRequestLocalization();

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
// Configure larger messages to allow upload of packages
builder.Services.Configure<HubOptions>(options =>
{
	options.MaximumReceiveMessageSize = 1024 * 1024 * appState.MaximumUploadSizeMB; // 1MB or use null
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


var pluginRoot = new PhysicalFileProvider(
	Path.Combine(app.Environment.ContentRootPath, @"plugins/_wwwroot"));
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
	FileProvider = pluginRoot,
	RequestPath = "/plugins"

});

app.UseAntiforgery();

app.UseOutputCache();

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

// Use DI to get the logger
var pluginManager = app.Services.GetRequiredService<PluginManager>();
await pluginManager.LoadPluginsAtStartup();

app.Run();
