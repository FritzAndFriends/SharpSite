public static class PostgresExtensions
{

	/// <summary>
	/// A collection of version information used by the containers in this app
	/// </summary>
	public static class VERSIONS
	{
		public const string POSTGRES = "17.2";
		public const string PGADMIN = "latest";
	}


	public static
		(IResourceBuilder<PostgresDatabaseResource> db,
		IResourceBuilder<ProjectResource> migrationSvc) AddPostgresServices(
		this IDistributedApplicationBuilder builder,
		bool testOnly = false)
	{

		var dbServer = builder.AddPostgres("database")
			.WithImageTag(VERSIONS.POSTGRES);

		if (!testOnly)
		{
			dbServer = dbServer.WithLifetime(ContainerLifetime.Persistent)
				.WithDataVolume($"{SharpSite.Data.Postgres.Constants.DBNAME}-data", false)
				.WithPgAdmin(config =>
				{
					config.WithImageTag(VERSIONS.PGADMIN);
					config.WithLifetime(ContainerLifetime.Persistent);
				});

		}
		else
		{
			dbServer = dbServer
				.WithLifetime(ContainerLifetime.Session);
		}

		var outdb = dbServer.AddDatabase(SharpSite.Data.Postgres.Constants.DBNAME);

		var migrationSvc = builder.AddProject<Projects.SharpSite_Data_Postgres_Migration>($"{SharpSite.Data.Postgres.Constants.DBNAME}migrationsvc")
			.WithReference(outdb)
			.WaitFor(dbServer);

		return (outdb, migrationSvc);

	}

}
