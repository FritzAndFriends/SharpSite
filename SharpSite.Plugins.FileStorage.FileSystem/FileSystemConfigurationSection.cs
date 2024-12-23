using SharpSite.Abstractions.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SharpSite.Plugins.FileStorage.FileSystem;

public class FileSystemConfigurationSection : ISharpSiteConfigurationSection
{
	public string SectionName { get; } = "FileSystemStorage";

	public Dictionary<string, string> Configuration { get; } = new();

	[DisplayName("Base Folder Name"), Required, MaxLength(100)]
	public string BaseFolderName { get; set; } = "UploadedFiles";

}