
namespace SharpSite.Abstractions.Base;

public interface ISharpSiteConfigurationSection
{

	string SectionName { get; }

	Task OnConfigurationChanged(ISharpSiteConfigurationSection? oldConfiguration, IPluginManager pluginManager);

}
