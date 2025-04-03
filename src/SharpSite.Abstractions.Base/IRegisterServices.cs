using Microsoft.Extensions.Hosting;

namespace SharpSite.Abstractions.Base;

/// <summary>
/// Interface for services that need to register services with the web application.
/// </summary>
public interface IRegisterServices
{

	IHostApplicationBuilder RegisterServices(IHostApplicationBuilder services, bool disableRetry = false);

}

public interface IManageDatabase
{
	/// <summary>
	/// Creates the database if it does not exist.
	/// </summary>
	void CreateDatabaseIfNotExists(string connectionString);

	/// <summary>
	/// Updates the database schema to the latest versions
	/// </summary>
	/// <returns></returns>
	Task UpdateDatabaseSchemaAsync(string connectionString);

}
