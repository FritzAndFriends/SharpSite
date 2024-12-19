using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace SharpSite.Plugins;

public class PluginAssemblyManager: IDisposable
{
	private bool disposed = false;
	private readonly Dictionary<string, PluginAssembly> _pluginAssemblies = new Dictionary<string, PluginAssembly>();

	public IReadOnlyDictionary<string, PluginAssembly> Assemblies => _pluginAssemblies;

	public void AddAssembly(PluginAssembly assembly)
	{
		if (!_pluginAssemblies.ContainsKey(assembly.Manifest.Id))
		{
			_pluginAssemblies.Add(assembly.Manifest.Id, assembly);
		}
		else 
		{
			var oldAssembly = _pluginAssemblies[assembly.Manifest.Id];
			oldAssembly.UnloadContext();
			_pluginAssemblies[assembly.Manifest.Id] = assembly;
			assembly.LoadContext();
		}
	}

	public void RemoveAssembly(PluginAssembly assembly)
	{
		if (_pluginAssemblies.ContainsKey(assembly.Manifest.Id))
		{
			var oldAssembly = _pluginAssemblies[assembly.Manifest.Id];
			oldAssembly.UnloadContext();
			_pluginAssemblies.Remove(assembly.Manifest.Id);
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposed)
		{
			foreach(var pluginAssembly in _pluginAssemblies.Values)
			{
				pluginAssembly.UnloadContext();
			}
			disposed = true;
		}
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}