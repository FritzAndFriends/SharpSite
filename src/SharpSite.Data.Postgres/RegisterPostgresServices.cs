using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharpSite.Abstractions;
using SharpSite.Abstractions.Base;

namespace SharpSite.Data.Postgres;

public class RegisterPostgresServices : IRegisterServices
{
	public IHostApplicationBuilder RegisterServices(IHostApplicationBuilder host, bool disableRetry = false)
	{

		host.Services.AddTransient<IPageRepository, PgPageRepository>();
		host.Services.AddTransient<IPostRepository, PgPostRepository>();
		host.AddNpgsqlDbContext<PgContext>(Constants.DBNAME, configure =>
		{
			configure.DisableRetry = disableRetry;
		});

		return host;

	}
}

public static class Constants
{

	public const string DBNAME = "SharpSite";

}