using Microsoft.Extensions.Hosting;

namespace SharpSite.Abstractions.Base;

/// <summary>
/// Interface for services that need to run at startup of the web application.
/// </summary>
public interface IRunAtStartup
{

	/// <summary>
	/// A method that run when the plugin is installed.
	/// </summary>
	Task RunOnInstall();

	/// <summary>
	/// A method that run when the plugin is updated
	/// </summary>
	Task RunOnUpdate();

	/// <summary>
	/// Executes a task during the uninstallation process.
	/// </summary>
	/// <returns>Returns a Task representing the asynchronous operation.</returns>
	Task RunOnUninstall();

	/// <summary>
	/// Method that runs at startup of the web application.
	/// </summary>
	/// <param name="app">The application being configured</param>
	Task<IHostApplicationBuilder> RunAtStartup(IHostApplicationBuilder app);


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
