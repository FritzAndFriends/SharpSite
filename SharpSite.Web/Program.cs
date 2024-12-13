using Microsoft.AspNetCore.Identity;
using SharpSite.Abstractions;
using SharpSite.Data.Postgres;
using SharpSite.Security.Postgres;
using SharpSite.Web;
using SharpSite.Web.Components;
using SharpSite.Web.Locales;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

Assembly.LoadFrom(
	Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.FullName,
		"Sample.FirstThemePlugin.dll")
	);

var pg = new RegisterPostgresServices();
pg.RegisterServices(builder);

var pgSecurity = new RegisterPostgresSecurityServices();
pgSecurity.RegisterServices(builder);

// add the custom localization features for the application framework
builder.ConfigureRequestLocalization();

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.Services.AddTransient<PluginManager>();

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

builder.Services.AddSingleton<ApplicationState>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
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
