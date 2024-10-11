using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharpSite.Abstractions;

namespace SharpSite.Data.Postgres;

public class RegisterPostgresServices : IRegisterServices
{
	public IHostApplicationBuilder RegisterServices(IHostApplicationBuilder host)
	{
		
		host.Services.AddScoped<IPostRepository, PgPostRepository>();
		
		return host;

	}
}