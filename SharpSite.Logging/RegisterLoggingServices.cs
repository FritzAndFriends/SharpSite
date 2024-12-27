using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SharpSite.Abstractions;

namespace SharpSite.Logging;

public class RegisterLoggingServices : IRegisterLoggingServices
{
	public IHostApplicationBuilder RegisterServices(IHostBuilder host, IHostApplicationBuilder hostApplication)
	{
		Log.Logger = new LoggerConfiguration()
				.WriteTo.Seq(Environment.GetEnvironmentVariable("SEQ_SERVER_API_ENDPOINT") ?? throw new ArgumentException("SEQ_SERVER_API_ENDPOINT is not set"))
				.CreateLogger();

		host.UseSerilog((context, loggerConfiguration) =>
		{
			loggerConfiguration.WriteTo.Console();
			loggerConfiguration.ReadFrom.Configuration(context.Configuration);
		});

		hostApplication.Services.AddSingleton(Log.Logger);

		host.UseSerilog((context, configuration) =>
				configuration.ReadFrom.Configuration(context.Configuration));

		return hostApplication;
	}

	public async Task RunAtStartup(WebApplication app)
	{
		app.UseSerilogRequestLogging();
		await Task.Yield();
	}
}

public static class Constants
{
	public const string Version = "1.0.0";
}