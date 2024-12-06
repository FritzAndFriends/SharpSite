using System.Security.Claims;

namespace SharpSite.Abstractions;

public interface IUserRepository
{

	Task<SharpSiteUser> GetUserAsync(ClaimsPrincipal user);

}