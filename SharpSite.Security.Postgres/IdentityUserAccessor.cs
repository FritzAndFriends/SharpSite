using Microsoft.AspNetCore.Identity;
using SharpSite.Abstractions;

namespace SharpSite.Security.Postgres;

internal sealed class IdentityUserAccessor(UserManager<PgSharpSiteUser> userManager, IdentityRedirectManager redirectManager)
{
	public async Task<SharpSiteUser> GetRequiredUserAsync(HttpContext context)
	{
		var user = await userManager.GetUserAsync(context.User);

		if (user is null)
		{
			redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
		}

		return user;
	}
}
