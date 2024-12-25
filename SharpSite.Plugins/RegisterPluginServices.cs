using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharpSite.Abstractions;

namespace SharpSite.Plugins;

public class RegisterPluginServices : IRegisterServices, IRunAtStartup
{
	public IHostApplicationBuilder RegisterServices(IHostApplicationBuilder builder, bool disableRetry = false)
	{
		builder.Services.AddSingleton<IPluginAssemblyManager, PluginAssemblyManager>();
		builder.Services.AddSingleton<IPluginManager, PluginManager>();
		builder.Services.AddSingleton<IThemeManager, ThemeManager>();
		return builder;
	}

	public async Task RunAtStartup(IServiceProvider services)
	{
		var pluginManager = services.GetRequiredService<IPluginManager>();
		await pluginManager.LoadAtStartup();

		var themeManager = services.GetRequiredService<IThemeManager>();
		await themeManager.LoadAtStartup();
	}
}