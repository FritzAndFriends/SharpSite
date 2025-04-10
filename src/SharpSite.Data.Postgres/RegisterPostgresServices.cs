using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharpSite.Abstractions;
using SharpSite.Abstractions.Base;

namespace SharpSite.Data.Postgres;

public class RegisterPostgresServices : IRunAtStartup
{
	public void CreateDatabaseIfNotExists(string connectionString)
	{

		// create an instance of the database if it does not exist using the entity framework context with the connection string passed in
		var optionsBuilder = new DbContextOptionsBuilder<PgContext>();
		optionsBuilder.UseNpgsql<PgContext>(connectionString);
		using var context = new PgContext(optionsBuilder.Options);
		context.Database.EnsureCreated();

	}


	public Task<IHostApplicationBuilder> RunAtStartup(IHostApplicationBuilder app)
	{
		// check if the database connection string is available
		if (string.IsNullOrEmpty(app.Configuration[$"Connectionstrings:{Constants.DBNAME}"]) {

			// check if AppSettings has the connection string

		}

		app.Services.AddTransient<IPageRepository, PgPageRepository>();
		app.Services.AddTransient<IPostRepository, PgPostRepository>();
		app.AddNpgsqlDbContext<PgContext>(Constants.DBNAME, configure =>
		{
			configure.DisableRetry = true;
		});

		return Task.FromResult(app);

	}

	public Task RunOnInstall()
	{
		throw new NotImplementedException();
	}

	public Task RunOnUninstall()
	{
		throw new NotImplementedException();
	}

	public Task RunOnUpdate()
	{
		throw new NotImplementedException();
	}

	public async Task UpdateDatabaseSchemaAsync(string connectionString)
	{
		// create an instance of the database if it does not exist using the entity framework context with the connection string passed in
		var optionsBuilder = new DbContextOptionsBuilder<PgContext>();
		optionsBuilder.UseNpgsql<PgContext>(connectionString);
		using var context = new PgContext(optionsBuilder.Options);
		await context.Database.MigrateAsync();
	}

}

public static class Constants
{

	public const string DBNAME = "SharpSite";

}