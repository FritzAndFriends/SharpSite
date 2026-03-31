using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace SharpSite.Plugins;

public class PluginAssemblyManager(ILogger<PluginAssemblyManager> logger): IDisposable
{
	private readonly ILogger<PluginAssemblyManager> _logger = logger;

	private bool disposed = false;
	private readonly ConcurrentDictionary<string, PluginAssembly> _pluginAssemblies = new();

	public IReadOnlyDictionary<string, PluginAssembly> Assemblies => _pluginAssemblies;

	public void AddAssembly(PluginAssembly assembly)
	{
		_logger.LogInformation("Assembly {AssemblyManifestId} being added", assembly.Manifest.Id);
		_pluginAssemblies.AddOrUpdate(
			assembly.Manifest.Id,
			key =>
			{
				_logger.LogInformation("Plugins does not have plugin assembly with id {AssemblyManifestId}", assembly.Manifest.Id);
				return assembly;
			},
			(key, existingAssembly) =>
			{
				_logger.LogInformation("Plugins does have plugin assembly with id {AssemblyManifestId}", assembly.Manifest.Id);
				existingAssembly.UnloadContext();
				return assembly;
			});
		assembly.LoadContext();
	}

	public void RemoveAssembly(PluginAssembly assembly)
	{
		if (_pluginAssemblies.TryRemove(assembly.Manifest.Id, out var removed))
		{
			removed.UnloadContext();
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