var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.SharpSite_Web>("webfrontend")
    .WithExternalHttpEndpoints();

builder.Build().Run();
