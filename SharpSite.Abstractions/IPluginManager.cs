
namespace SharpSite.Abstractions;

public interface IPluginManager: IStartupManager
{
	IReadOnlyDictionary<string, IPluginManifest> Plugins { get; }

	void AddPlugin(string pluginName, IPluginManifest manifest);
}
