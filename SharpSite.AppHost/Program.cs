var builder = DistributedApplication.CreateBuilder(args);

var dbServer = builder.AddPostgres("database")
	.WithPgAdmin();

var db = dbServer.AddDatabase(SharpSite.Data.Postgres.Constants.DBNAME);


builder.AddProject<Projects.SharpSite_Web>("webfrontend")
	.WithReference(db)
	.WithExternalHttpEndpoints();

builder.Build().Run();
