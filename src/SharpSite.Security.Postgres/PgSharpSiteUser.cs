using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharpSite.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace SharpSite.Security.Postgres;

public class PgSharpSiteUser : IdentityUser
{

	[PersonalData, Required, MaxLength(50)]
	public required string DisplayName { get; set; }

	public static explicit operator SharpSiteUser(PgSharpSiteUser user) =>
			new(user.Id, user.UserName, user.Email)
			{
				DisplayName = user.DisplayName
			};

	public static explicit operator PgSharpSiteUser(SharpSiteUser user) =>
		new()
		{
			Id = user.Id,
			DisplayName = user.DisplayName,
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