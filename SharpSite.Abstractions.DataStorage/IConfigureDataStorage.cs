using SharpSite.Abstractions.Base;

namespace SharpSite.Abstractions.DataStorage;

public interface IConfigureDataStorage
{

	SortedDictionary<int, string> ConfigurationFields { get; }

	void ConfigureDataStorage(IApplicationStateModel appState, Dictionary<string, string> configuration);

}
