using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SharpSite.Abstractions.Base;
using SharpSite.Abstractions.DataStorage;
using System.Data.Common;

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

	public void ParseConnectionString(string connectionString, Dictionary<string, string> configuration)
	{

		var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };
		
		if (builder.TryGetValue("Host", out var host))
			configuration["Server Name"] = host?.ToString() ?? string.Empty;
		
		if (builder.TryGetValue("Database", out var database))
			configuration["Database Name"] = database?.ToString() ?? string.Empty;
		
		if (builder.TryGetValue("Username", out var username))
			configuration["User Name"] = username?.ToString() ?? string.Empty;
		
		if (builder.TryGetValue("Password", out var password))
			configuration["Password"] = password?.ToString() ?? string.Empty;
		
		if (builder.TryGetValue("Port", out var port))
			configuration["Port"] = port?.ToString() ?? "5432";
	}

	public async Task CreateNewDataStorage(IApplicationStateModel appState)
	{

		var context = new PgContext(appState);
		await context.Database.MigrateAsync();

	}

	public async Task UpdateDataStorage(IApplicationStateModel appState)
	{

		// This method is called when a data storage plugin is updated
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
