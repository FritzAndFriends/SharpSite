using Newtonsoft.Json;
using SharpSite.Abstractions.Base;

namespace SharpSite.Web;

public static class SharpsiteConfigurationExtensions
{

	public static ISharpSiteConfigurationSection CloneSection(this ApplicationState appState, string sectionName)
	{

		var theType = appState.ConfigurationSections[sectionName].GetType();
		var json = JsonConvert.SerializeObject(appState.ConfigurationSections[sectionName],
			new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto,
			});

		return (ISharpSiteConfigurationSection)JsonConvert.DeserializeObject(
			json,
			theType,
			new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto,
			})!;

	}


}