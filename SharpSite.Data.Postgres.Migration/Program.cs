using SharpSite.Data.Postgres;
using SharpSite.Data.Postgres.Migration;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
var pg = new RegisterPostgresServices();
pg.RegisterServices(builder);

builder.Services.AddHostedService<Worker>();


builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

var host = builder.Build();
host.Run();
