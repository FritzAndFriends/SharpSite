using SharpSite.Data.Postgres;
using SharpSite.Data.Postgres.Migration;
using SharpSite.Security.Postgres;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
var pg = new RegisterPostgresServices();
pg.RegisterServices(builder, disableRetry: true);

RegisterPostgresSecurityServices.ConfigurePostgresDbContext(builder, disableRetry: true);

builder.Services.AddHostedService<Worker>();


builder.Services.AddOpenTelemetry()
		.WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

var host = builder.Build();
host.Run();
