var builder = DistributedApplication.CreateBuilder(args);

var dbServer = builder.AddPostgres("database")
	.WithPgAdmin();

var db = dbServer.AddDatabase(SharpSite.Data.Postgres.Constants.DBNAME);


var migrationSvc = builder.AddProject<Projects.SharpSite_Data_Postgres_Migration>("migrationsvc")
	.WithReference(db)
	.WaitFor(dbServer);

builder.AddProject<Projects.SharpSite_Web>("webfrontend")
	.WithReference(db)
	.WaitFor(migrationSvc)
	.WithExternalHttpEndpoints();

builder.Build().Run();
