namespace SharpSite.Abstractions.Base;

/// <summary>
/// Interface for services that need to run at startup of the web application.
/// </summary>
public interface IRunAtStartup
{
	Task RunAtStartup(IServiceProvider services);
}

public interface IHasEndpoints
{
	void MapEndpoints(IServiceProvider services);
}


public interface IPluginManager
{

	Task<DirectoryInfo> CreateDirectoryInPluginsFolder(string name);
	DirectoryInfo GetDirectoryInPluginsFolder(string name);
	Task<DirectoryInfo> MoveDirectoryInPluginsFolder(string oldName, string newName);

}
