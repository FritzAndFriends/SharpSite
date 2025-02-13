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
	.WaitForCompletion(migrationSvc)
	.WithRunE2eTestsCommand()
	.WithExternalHttpEndpoints();

if (testOnly)
{
	// start the site with runasync and watch for a file to be created called 'stop-aspire' 
	// to stop the site
	var theSite = builder.Build();
	var fileSystemWatcher = new FileSystemWatcher(".", "stop-aspire")
	{
		NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime
	};

	fileSystemWatcher.Created += async (sender, e) =>
	{
		if (e.Name == "stop-aspire")
		{
			Console.WriteLine("Stopping the site");
			await theSite.StopAsync();
			fileSystemWatcher.Dispose();
		}
	};

	fileSystemWatcher.EnableRaisingEvents = true;

	Console.WriteLine("Starting the site in test mode");
	await theSite.RunAsync();

}
else
{
	builder.Build().Run();

}


