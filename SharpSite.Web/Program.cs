using SharpSite.Data.Postgres;
using SharpSite.Web;
using SharpSite.Web.Components;

var builder = WebApplication.CreateBuilder(args);

var pg = new RegisterPostgresServices();
pg.RegisterServices(builder);

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
	var supportedCultures = new[] { "en-US", "nl-NL" };
	options.SetDefaultCulture(supportedCultures[0])
		.AddSupportedCultures(supportedCultures)
		.AddSupportedUICultures(supportedCultures);
});

builder.Services.AddLocalization(options =>
{
	options.ResourcesPath = "Locales";
});

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

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
    .AddInteractiveServerRenderMode();

app.MapSiteMap();
app.MapRobotsTxt();
app.MapRssFeed();
app.MapDefaultEndpoints();

app.UseRequestLocalization();

app.Run();
