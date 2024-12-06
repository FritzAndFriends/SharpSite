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
		var userManager = scope.ServiceProvider.GetRequiredService<UserManager<PgSharpSiteUser>>();
		var pgUsers = await userManager.Users.ToListAsync();
		return pgUsers.Select(u => (SharpSiteUser)u).ToList();
	}

}