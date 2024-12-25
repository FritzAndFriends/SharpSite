using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.FileProviders;
using SharpSite.Data.Postgres;
using SharpSite.Security.Postgres;
using SharpSite.Web;
using SharpSite.Web.Components;
using SharpSite.Web.Locales;
using SharpSite.Plugins;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using SharpSite.Abstractions;

var builder = WebApplication.CreateBuilder(args);

PluginManager.Initialize();

// LoadStateAtStartup plugins for postgres
#region Postgres Plugins
var pg = new RegisterPostgresServices();
pg.RegisterServices(builder);

var pgSecurity = new RegisterPostgresSecurityServices();
pgSecurity.RegisterServices(builder);
#endregion

#region Theme / Generic Plugins
var pluginManagers = new RegisterPluginServices();
pluginManagers.RegisterServices(builder);
#endregion

// add the custom localization features for the application framework
builder.ConfigureRequestLocalization();

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.Services
	.AddOptions<HubOptions>()
	.Bind(builder.Configuration.GetSection(ConfigurationSections.HubOptionsConfigurationSection));

// Configure larger messages to allow upload of packages
builder.Services
	.Configure<HubOptions>(options =>
	{
		if (options.MaximumReceiveMessageSize == null)
		{
			options.MaximumReceiveMessageSize = 1024 * 1024 * 10; // 10MB or use null
		}
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
await pluginManagers.RunAtStartup(app.Services);

app.Run();
