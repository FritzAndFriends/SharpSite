using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SharpSite.Security.Postgres;

public class PgSharpSiteUser : IdentityUser
{
}

public class PgSecurityContext : IdentityDbContext<PgSharpSiteUser>
{
	public PgSecurityContext(DbContextOptions<PgSecurityContext> options)
		: base(options)
	{
	}

}