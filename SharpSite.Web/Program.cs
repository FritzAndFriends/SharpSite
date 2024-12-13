using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using SharpSite.Abstractions;
using SharpSite.Data.Postgres;
using SharpSite.Security.Postgres;
using SharpSite.Web;
using SharpSite.Web.Components;
using SharpSite.Web.Locales;

var builder = WebApplication.CreateBuilder(args);

// Load plugins
var appState = new ApplicationState();
await PluginManager.LoadPluginsAtStartup(appState);

var pg = new RegisterPostgresServices();
pg.RegisterServices(builder);

var pgSecurity = new RegisterPostgresSecurityServices();
pgSecurity.RegisterServices(builder);

// add the custom localization features for the application framework
builder.ConfigureRequestLocalization();

// Add service defaults & Aspire components.
builder.AddServiceDefaults();


// Add services to the container.
builder.Services.AddRazorComponents()
		.AddInteractiveServerComponents()
		.AddHubOptions(options =>
		{
			options.MaximumReceiveMessageSize = 1024 * 1024 * 10; // 10 MB
			options.EnableDetailedErrors = true;
		});

builder.Services.AddOutputCache();
builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IEmailSender<SharpSiteUser>, IdentityNoOpEmailSender>();

builder.Services.AddSingleton(appState);
builder.Services.AddTransient<PluginManager>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();


app.UseStaticFiles(new StaticFileOptions
{
	FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "wwwroot")),
	RequestPath = ""
});

// create the plugins folder if it doesn't exist
Directory.CreateDirectory(Path.Combine(app.Environment.ContentRootPath, "plugins"));
Directory.CreateDirectory(Path.Combine(app.Environment.ContentRootPath, "plugins/_wwwroot"));

app.UseStaticFiles(new StaticFileOptions
{
	FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "plugins/_wwwroot")),
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

app.Run();
