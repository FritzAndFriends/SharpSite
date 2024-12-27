using Microsoft.Extensions.Hosting;

namespace SharpSite.Abstractions;

/// <summary>
/// Interface for services that need to register services with the web application.
/// </summary>
public interface IRegisterServices
{

	IHostApplicationBuilder RegisterServices(IHostApplicationBuilder builder, bool disableRetry = false);

}

/// <summary>
/// Interface for services that need to register services with the web application.
/// </summary>
public interface IRegisterLoggingServices
{

	IHostApplicationBuilder RegisterServices(IHostBuilder host, IHostApplicationBuilder hostApplication);

}