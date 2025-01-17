using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharpSite.Abstractions;
using System.Security.Claims;

namespace SharpSite.Security.Postgres;

public class UserRepository(IServiceProvider services) : IUserRepository
{

	private SharpSiteUser CurrentUser = null!;

	public async Task<SharpSiteUser> GetUserAsync(ClaimsPrincipal user)
	{

		if (CurrentUser is null)
		{

			using var scope = services.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<PgSharpSiteUser>>();

			var pgUser = await userManager.GetUserAsync(user);
			if (pgUser is null) return null!;

			CurrentUser = (SharpSiteUser)pgUser;
		}

		return CurrentUser;

	}

	public async Task<IEnumerable<SharpSiteUser>> GetAllUsersAsync()
	{
		using var scope = services.CreateScope();
		var userManager = scope.ServiceProvider.GetRequiredService<PgSecurityContext>();
		var pgUsers = await userManager.Users
			.GroupJoin(userManager.UserRoles, u => u.Id, ur => ur.UserId, (u, urs) => new { u, urs })
			.SelectMany(
				x => x.urs.DefaultIfEmpty(),
				(x, ur) => new { x.u, ur }
			)
			.GroupJoin(userManager.Roles, x => x.ur!.RoleId, r => r.Id, (x, rs) => new { x.u, x.ur, rs })
			.SelectMany(
				x => x.rs.DefaultIfEmpty(),
				(x, r) => new SharpSiteUser(x.u.Id, x.u.UserName, x.u.Email)
				{
					DisplayName = x.u.DisplayName,
					PhoneNumber = x.u.PhoneNumber,
					Role = r != null ? r.Name : "No Role Assigned"
				}
			).ToListAsync();

		return pgUsers;
	}

	public async Task UpdateRoleForUserAsync(SharpSiteUser user)
	{

		if (user is null) return;

		using var scope = services.CreateScope();
		var userManager = scope.ServiceProvider.GetRequiredService<UserManager<PgSharpSiteUser>>();

		var existingUser = userManager.Users.FirstOrDefault(u => u.Id == user.Id);
		if (existingUser is null) return;

		var existingRole = (await userManager.GetRolesAsync(existingUser)).FirstOrDefault();
		if (existingRole is not null) await userManager.RemoveFromRoleAsync(existingUser, existingRole);

		if (user.Role is not null)
		await userManager.AddToRoleAsync(existingUser, user.Role);
		

	}
}