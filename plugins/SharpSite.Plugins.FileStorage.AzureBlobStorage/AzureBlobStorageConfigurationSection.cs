using SharpSite.Abstractions.Base;

namespace SharpSite.Plugins.FileStorage.AzureBlobStorage;

public class AzureBlobStorageConfigurationSection : ISharpSiteConfigurationSection
{
	public string SectionName { get; } = "Azure Blob Storage";
	public string? ConnectionString { get; set; }

	public Task OnConfigurationChanged(ISharpSiteConfigurationSection? oldConfiguration, IPluginManager pluginManager)
	{
		throw new NotImplementedException();
	}
}
