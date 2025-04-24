using SharpSite.Abstractions.Base;

namespace SharpSite.Abstractions.DataStorage;

public interface IConfigureDataStorage
{

	/// <summary>
	/// A sorted collection of key-value pairs where keys are integers and values are strings. It provides a way to access
	/// configuration field labels in a sorted manner.
	/// </summary>
	SortedDictionary<int, string> ConfigurationFields { get; }

	/// <summary>
	/// This method is called when a new data storage plugin is installed.
	/// </summary>
	/// <returns></returns>
	Task CreateNewDataStorage(IApplicationStateModel appState);

	/// <summary>
	/// Formats a connection string using the provided key-value pairs. It constructs a string suitable for database
	/// connections.
	/// </summary>
	/// <param name="connectionStringParts">Contains key-value pairs that represent the components of the connection string.</param>
	/// <returns>Returns a formatted connection string based on the provided components.</returns>
	string FormatConnectionString(Dictionary<string, string> connectionStringParts);

	/// <summary>
	/// This method is called when a data storage plugin is updated.
	/// </summary>
	/// <returns></returns>
	Task UpdateDataStorage(IApplicationStateModel appState);

}
