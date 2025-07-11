using System.Reflection;

namespace SharpSite.Plugins;

public class PluginAssembly(PluginManifest pluginMainfest, Plugin plugin)
{
	private readonly Plugin _plugin = plugin;
	private readonly PluginManifest _pluginMainfest = pluginMainfest;
	private PluginAssemblyLoadContext? _loadContext;
	public PluginAssemblyLoadContext? LoadContextInstance => _loadContext;
	private Assembly? _assembly;

	public Assembly? Assembly => _assembly;

	public PluginManifest Manifest => _pluginMainfest;

	public void LoadContext(string? mainAssemblyPath = null)
	{
		if (_loadContext is not null) return;
		_loadContext = new PluginAssemblyLoadContext(mainAssemblyPath);
		_assembly = _loadContext.Load(_plugin.Bytes);
	}

	public void UnloadContext()
	{
		if (_loadContext is null) return;
		_loadContext.Unload();
		_loadContext = null;
		GC.Collect();
		GC.WaitForPendingFinalizers();
	}
}