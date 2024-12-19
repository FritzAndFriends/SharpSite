using Microsoft.AspNetCore.Components;
using SharpSite.Abstractions;
using System.Reflection;
using System.Runtime.Loader;

namespace SharpSite.Plugins;

public class PluginAssembly
{
	private readonly Plugin _plugin;
	private readonly PluginManifest _pluginMainfest;
	private PluginAssemblyLoadContext? _loadContext;
	private Assembly? _assembly;

	public Assembly? Assembly => _assembly;

	public PluginManifest Manifest => _pluginMainfest;

	public PluginAssembly(PluginManifest pluginMainfest, Plugin plugin)
	{
		_plugin = plugin;
		_pluginMainfest = pluginMainfest;
	}

	public void LoadContext()
	{
		if (_loadContext != null) return;
		_loadContext = new PluginAssemblyLoadContext();
		_assembly = _loadContext.Load(_plugin.Bytes);
	}

	public void UnloadContext()
	{
		if (_loadContext == null) return;
		_loadContext.Unload();
		_loadContext = null;
		GC.Collect();
		GC.WaitForPendingFinalizers();
	}
}