using System.Text.Json;
using SharpSite.Abstractions.Base;

namespace SharpSite.Web;

public static class SharpsiteConfigurationExtensions
{

	public static ISharpSiteConfigurationSection CloneSection(this ApplicationState appState, string sectionName)
	{
		var section = appState.ConfigurationSections[sectionName];
		var concreteType = section.GetType();

		var json = JsonSerializer.Serialize(section, concreteType);
		return (ISharpSiteConfigurationSection)JsonSerializer.Deserialize(json, concreteType)!;
	}

}