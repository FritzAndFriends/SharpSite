using SharpSite.Abstractions.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SharpSite.Plugins.FileStorage.FileSystem;

public class FileSystemConfigurationSection : ISharpSiteConfigurationSection
{
	public string SectionName { get; } = "FileSystem Storage";

	[DisplayName("Base Folder Name"), Required, MaxLength(500)]
	public string BaseFolderName { get; set; } = "UploadedFiles";

	public async Task OnConfigurationChanged(ISharpSiteConfigurationSection? oldConfiguration, IPluginManager pluginManager)
	{

		var oldConfig = oldConfiguration as FileSystemConfigurationSection;

		// check if the base folder name has changed, and if so, move the folder
		if (oldConfig is not null && oldConfig.BaseFolderName != BaseFolderName)
		{
			await pluginManager.MoveDirectoryInPluginsFolder(oldConfig.BaseFolderName, BaseFolderName);
		}
		else
		{
			await pluginManager.CreateDirectoryInPluginsFolder(BaseFolderName);
		}

	}

}