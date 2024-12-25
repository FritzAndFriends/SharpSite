
namespace SharpSite.Abstractions;

public interface IPluginManager: IStartupManager
{
	IReadOnlyDictionary<string, IPluginManifest> Plugins { get; }

	void AddPlugin(string pluginName, IPluginManifest manifest);

	IPluginManifest? Manifest { get; }

	void HandleUploadedPlugin(IPlugin plugin);

	void ValidatePlugin(string pluginName);

	Task SavePlugin();
}
