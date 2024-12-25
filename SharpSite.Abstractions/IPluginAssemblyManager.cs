namespace SharpSite.Abstractions;

public interface IPluginAssemblyManager
{
	void AddAssembly(IPluginAssembly pluginAssembly);
	void RemoveAssembly(IPluginAssembly pluginAssembly);
}