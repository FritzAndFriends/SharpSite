using System.Security.Claims;

namespace SharpSite.Abstractions;

public interface IUserRepository
{

	Task<SharpSiteUser> GetUserAsync(ClaimsPrincipal user);

	Task<IEnumerable<SharpSiteUser>> GetAllUsersAsync();

	Task UpdateRoleForUserAsync(SharpSiteUser user);

}