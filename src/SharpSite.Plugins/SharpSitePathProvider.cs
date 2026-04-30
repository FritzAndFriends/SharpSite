namespace SharpSite.Plugins;

public static class SharpSitePathProvider
{
	private static string _ContentRootPath = Directory.GetCurrentDirectory();

	public static void Initialize(string? contentRootPath)
	{
		if (!string.IsNullOrWhiteSpace(contentRootPath))
		{
			_ContentRootPath = contentRootPath;
		}
	}

	public static string ContentRootPath => _ContentRootPath;

	public static string PluginsRootPath => Path.Combine(_ContentRootPath, "plugins");

	public static string UploadedPluginsRootPath => Path.Combine(PluginsRootPath, "_uploaded");

	public static string PluginWebRootPath => Path.Combine(PluginsRootPath, "_wwwroot");

	public static string DefaultPluginsRootPath => Path.Combine(_ContentRootPath, "defaultplugins");

	public static string ApplicationStatePath => Path.Combine(PluginsRootPath, "applicationState.json");

	public static string AssemblyHashRegistryPath => Path.Combine(PluginsRootPath, "_assembly-hashes.json");

	public static string GetPluginInstallationPath(string pluginFolderName) => Path.Combine(PluginsRootPath, pluginFolderName);

	public static string GetPluginPrivateDirectoryPath(string directoryName) => Path.Combine(PluginsRootPath, "_" + directoryName);
}
