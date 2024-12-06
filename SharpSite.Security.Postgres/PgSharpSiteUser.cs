using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharpSite.Abstractions;

namespace SharpSite.Security.Postgres;

public class PgSharpSiteUser : IdentityUser
{

	public static explicit operator SharpSiteUser(PgSharpSiteUser user) =>
			new(user.Id, user.UserName, user.Email);

	public static explicit operator PgSharpSiteUser(SharpSiteUser user) =>
		new()
		{
			Id = user.Id,
			UserName = user.UserName,
			Email = user.Email
		};

}

public class PgSecurityContext : IdentityDbContext<PgSharpSiteUser>
{
	public PgSecurityContext(DbContextOptions<PgSecurityContext> options)
		: base(options)
	{
	}

}