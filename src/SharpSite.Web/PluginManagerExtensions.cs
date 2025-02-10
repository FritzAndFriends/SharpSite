using Microsoft.Extensions.FileProviders;
using SharpSite.Plugins;

namespace SharpSite.Web;

public static class PluginManagerExtensions
{
	/// <summary>
	/// Configure application state and the PluginManager
	/// </summary>
	public static ApplicationState AddPluginManagerAndAppState(this WebApplicationBuilder builder)
	{

		PluginManager.Initialize();

		var appState = new ApplicationState();
		builder.Services.AddSingleton(appState);
		builder.Services.AddSingleton<PluginAssemblyManager>();
		builder.Services.AddSingleton<PluginManager>();

		return appState;

	}

	public static WebApplication ConfigurePluginFileSystem(this WebApplication app)
	{

		var pluginRoot = new PhysicalFileProvider(
		 Path.Combine(app.Environment.ContentRootPath, @"plugins/_wwwroot"));
		app.UseStaticFiles();
		app.UseStaticFiles(new StaticFileOptions()
		{
			FileProvider = pluginRoot,
			RequestPath = "/plugins"
		});

		return app;

	}

	public static async Task<PluginManager> ActivatePluginManager(this WebApplication app, ApplicationState appState)
	{

		var pluginManager = app.Services.GetRequiredService<PluginManager>();
		await pluginManager.LoadPluginsAtStartup();
		await appState.Load(app.Services);

		return pluginManager;

	}

}
