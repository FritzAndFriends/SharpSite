var builder = DistributedApplication.CreateBuilder(args);

var testOnly = false;

foreach (var arg in args)
{
    if (arg.StartsWith("--testonly"))
    {
        var parts = arg.Split('=');
        if (parts.Length == 2 && bool.TryParse(parts[1], out var result))
        {
            testOnly = result;
        }
    }
}

var (db, migrationSvc) = builder.AddPostgresServices(testOnly);

builder.AddProject<Projects.SharpSite_Web>("webfrontend")
	.WithReference(db)
	.WaitFor(migrationSvc)
	.WithExternalHttpEndpoints();

builder.Build().Run();
