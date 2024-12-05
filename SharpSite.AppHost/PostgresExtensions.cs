public static class PostgresExtensions
{

	public static
		(IResourceBuilder<PostgresDatabaseResource> db,
		IResourceBuilder<ProjectResource> migrationSvc) AddPostgresServices(
		this IDistributedApplicationBuilder builder)
	{

		var dbServer = builder.AddPostgres("database")
			.WithDataVolume($"{SharpSite.Data.Postgres.Constants.DBNAME}-data", false)
			.WithLifetime(ContainerLifetime.Persistent)
			.WithPgAdmin(config =>
			{
				// config.WithImageTag("latest");
				config.WithLifetime(ContainerLifetime.Persistent);
			});

		var outdb = dbServer.AddDatabase(SharpSite.Data.Postgres.Constants.DBNAME);

		var migrationSvc = builder.AddProject<Projects.SharpSite_Data_Postgres_Migration>($"{SharpSite.Data.Postgres.Constants.DBNAME}migrationsvc")
			.WithReference(outdb)
			.WaitFor(dbServer);

		return (outdb, migrationSvc);

	}

}
