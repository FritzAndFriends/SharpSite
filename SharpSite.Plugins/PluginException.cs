using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharpSite.Abstractions;
using System.Runtime.Serialization;

namespace SharpSite.Plugins;

public class PluginException : Exception
{
	public PluginException() { }
	public PluginException(string message) : base(message)	{ }
	public PluginException(Exception exception, string message) : base(message, exception)	{ }
}

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

		var applicationStateManager = services.GetRequiredService<IThemeManager>();
		await applicationStateManager.LoadAtStartup();
	}
}