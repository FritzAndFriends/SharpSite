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
		this IDistributedApplicationBuilder builder)
	{

		var dbServer = builder.AddPostgres("database")
			.WithImageTag(VERSIONS.POSTGRES)
			.WithDataVolume($"{SharpSite.Data.Postgres.Constants.DBNAME}-data54", false)
			.WithLifetime(ContainerLifetime.Persistent)
			.WithPgAdmin(config =>
			{
				config.WithImageTag(VERSIONS.PGADMIN);
				config.WithLifetime(ContainerLifetime.Persistent);
			});

		var outdb = dbServer.AddDatabase(SharpSite.Data.Postgres.Constants.DBNAME);

		var migrationSvc = builder.AddProject<Projects.SharpSite_Data_Postgres_Migration>($"{SharpSite.Data.Postgres.Constants.DBNAME}migrationsvc")
			.WithReference(outdb)
			.WaitFor(dbServer);

		return (outdb, migrationSvc);

	}

}
