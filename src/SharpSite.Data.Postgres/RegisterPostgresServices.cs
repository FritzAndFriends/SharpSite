using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharpSite.Abstractions;
using SharpSite.Abstractions.Base;

namespace SharpSite.Data.Postgres;

public class RegisterPostgresServices : IRegisterServices, IManageDatabase
{
	public void CreateDatabaseIfNotExists(string connectionString)
	{
		
		// create an instance of the database if it does not exist using the entity framework context with the connection string passed in
		var optionsBuilder = new DbContextOptionsBuilder<PgContext>();
		optionsBuilder.UseNpgsql<PgContext>(connectionString);
		using (var context = new PgContext(optionsBuilder.Options))
		{
			context.Database.EnsureCreated();
		}

	}


	public IHostApplicationBuilder RegisterServices(IHostApplicationBuilder host, bool disableRetry = false)
	{

		host.Services.AddTransient<IPageRepository, PgPageRepository>();
		host.Services.AddTransient<IPostRepository, PgPostRepository>();
		host.Services.AddTransient<IManageDatabase, RegisterPostgresServices>();
		host.AddNpgsqlDbContext<PgContext>(Constants.DBNAME, configure =>
		{
			configure.DisableRetry = disableRetry;
		});

		return host;

	}

	public async Task UpdateDatabaseSchemaAsync(string connectionString)
	{
		// create an instance of the database if it does not exist using the entity framework context with the connection string passed in
		var optionsBuilder = new DbContextOptionsBuilder<PgContext>();
		optionsBuilder.UseNpgsql<PgContext>(connectionString);
		using (var context = new PgContext(optionsBuilder.Options))
		{
			await context.Database.MigrateAsync();
		}
	}

}

public static class Constants
{

	public const string DBNAME = "SharpSite";

}