using Microsoft.AspNetCore.Components;
using SharpSite.Abstractions;
using System.Reflection;
using System.Runtime.Loader;

namespace SharpSite.Plugins;

public class PluginAssembly : IPluginAssembly
{
	private readonly IPlugin _plugin;
	private readonly IPluginManifest _pluginMainfest;
	private PluginAssemblyLoadContext? _loadContext;
	private Assembly? _assembly;

	public Assembly? Assembly => _assembly;

	private IPluginManifest Manifest => _pluginMainfest;

	IPluginManifest IPluginAssembly.Manifest => Manifest;

	public PluginAssembly(IPluginManifest pluginMainfest, IPlugin plugin)
	{
		_plugin = plugin;
		_pluginMainfest = pluginMainfest;
	}

	public void LoadContext()
	{
		if (_loadContext != null) return;
		_loadContext = new PluginAssemblyLoadContext();
		_assembly = _loadContext.Load(_plugin.Bytes.ToArray());
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