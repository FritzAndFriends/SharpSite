using Microsoft.EntityFrameworkCore;
using SharpSite.Abstractions.Base;
using SharpSite.Abstractions.DataStorage;

namespace SharpSite.Plugins.Data.Postgres;

[RegisterPlugin(PluginServiceLocatorScope.Transient, PluginRegisterType.DataStorage_Configuration)]
public class Configure : IConfigureDataStorage
{
	public Dictionary<string, string> ConfigurationFields => new()
	{
		{ "Server Name", "" },
		{ "Database Name", "" },
		{ "User Name", "" },
		{ "Password", "" },
		{ "Port", "5432" }
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

	public async Task CreateNewDataStorage(IApplicationStateModel appState)
	{

		var context = new PgContext(appState);
		await context.Database.MigrateAsync();

	}

	public async Task UpdateDataStorage(IApplicationStateModel appState)
	{

		// This method is called when a data storage plugin is updateds
		var context = new PgContext(appState);

		// This is a no-op if the database is already created and up to date.
		await context.Database.MigrateAsync();

	}

	public bool TestConnection(Dictionary<string, string> connectionStringParts, out string errorMessage)
	{

		var connectionString = FormatConnectionString(connectionStringParts);
		var context = new PgContext(connectionString);
		errorMessage = string.Empty;
		try
		{
			return context.Database.CanConnect();
		}
		catch (Exception ex)
		{
			errorMessage = ex.Message;
			return false;
		}
		finally
		{
			context.Dispose();
		}


	}
}
