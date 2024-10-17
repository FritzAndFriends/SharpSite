var builder = DistributedApplication.CreateBuilder(args);

var (db, migrationSvc) = builder.AddPostgresServices();

builder.AddProject<Projects.SharpSite_Web>("webfrontend")
	.WithReference(db)
	.WaitFor(migrationSvc)
	.WithExternalHttpEndpoints();

builder.Build().Run();
