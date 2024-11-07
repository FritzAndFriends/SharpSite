using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharpSite.Abstractions;

namespace SharpSite.Data.Postgres;

public class RegisterPostgresServices : IRegisterServices
{
	public IHostApplicationBuilder RegisterServices(IHostApplicationBuilder host)
	{
		
		host.Services.AddTransient<IPageRepository, PgPageRepository>();
		host.Services.AddTransient<IPostRepository, PgPostRepository>();
		host.AddNpgsqlDbContext<PgContext>(Constants.DBNAME);
		
		return host;

	}
}

public static class Constants
{

	public const string DBNAME = "SharpSite";

}