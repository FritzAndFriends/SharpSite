using Microsoft.Extensions.Hosting;

namespace SharpSite.Abstractions;

public interface IRegisterServices
{

	IHostApplicationBuilder RegisterServices(IHostApplicationBuilder services, bool disableRetry = false);

}