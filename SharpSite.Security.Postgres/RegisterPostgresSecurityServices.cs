using Microsoft.Extensions.Hosting;
using SharpSite.Abstractions;

namespace SharpSite.Security.Postgres;

public class RegisterPostgresSecurityServices : IRegisterServices
{
	public IHostApplicationBuilder RegisterServices(IHostApplicationBuilder services, bool disableRetry = false)
	{
		
		
		
		return services;

	}
}
