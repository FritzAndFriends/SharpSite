using SharpSite.Abstractions.Base;
using SharpSite.Abstractions.DataStorage;

namespace SharpSite.Plugins.Data.Postgres;

[RegisterPlugin(PluginServiceLocatorScope.Transient, PluginRegisterType.DataStorage_Configuration)]
public class Configure : IConfigureDataStorage
{
	public SortedDictionary<int, string> ConfigurationFields => new()
	{
		{ 1, "Server Name" },
		{ 2, "Database Name" },
		{ 3, "User Name" },
		{ 4, "Password" },
		{ 5, "Port" }
	};

	public string FormatConnectionString(Dictionary<string, string> connectionStringParts)
	{
		var serverName = connectionStringParts["Server Name"];
		var databaseName = connectionStringParts["Database Name"];
		var userName = connectionStringParts["User Name"];
		var password = connectionStringParts["Password"];
		var port = connectionStringParts.ContainsKey("Port") ? connectionStringParts["Port"] : "5432";
		return $"Host={serverName};Database={databaseName};Username={userName};Password={password};Port={port}";
	}

	public Task CreateNewDataStorage(IApplicationStateModel appState)
	{
		var connectionString = appState.GetConfigurationByName(ApplicationStateKeys.ContentConnectionString);

		return Task.CompletedTask;

	}

	public Task UpdateDataStorage(IApplicationStateModel appState)
	{
		var connectionString = appState.GetConfigurationByName(ApplicationStateKeys.ContentConnectionString);

		return Task.CompletedTask;

	}
}
