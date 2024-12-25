namespace SharpSite.Abstractions;

public interface IPluginAssembly
{
	IPluginManifest Manifest { get; }

	void LoadContext();
	void UnloadContext();
}