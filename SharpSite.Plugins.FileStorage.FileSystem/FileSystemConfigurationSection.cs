using SharpSite.Abstractions.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SharpSite.Plugins.FileStorage.FileSystem;

public class FileSystemConfigurationSection : ISharpSiteConfigurationSection
{
	public string SectionName { get; } = "FileSystem Storage";

	public Dictionary<string, string> Configuration { get; } = new();

	[DisplayName("Base Folder Name"), Required, MaxLength(500)]
	public string BaseFolderName { get; set; } = "UploadedFiles";

}