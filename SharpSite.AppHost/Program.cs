var builder = DistributedApplication.CreateBuilder(args);

var (db, migrationSvc) = builder.AddPostgresServices();

var seq = builder.AddSeqServices();

builder.AddProject<Projects.SharpSite_Web>("webfrontend")
	.WithReference(db)
	.WithReference(seq)
	.WaitFor(migrationSvc)
	.WithExternalHttpEndpoints();

builder.Build().Run();
