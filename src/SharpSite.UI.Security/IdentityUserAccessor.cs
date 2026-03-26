using Microsoft.AspNetCore.Http;
using SharpSite.UI.Security.Services;

namespace SharpSite.UI.Security;

internal sealed class IdentityUserAccessor(IUserManager userManager, IdentityRedirectManager redirectManager)
{
    public async Task<ISharpSiteUser> GetRequiredUserAsync(HttpContext context)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null)
        {
            redirectManager.RedirectTo("Account/InvalidUser");
        }

        return user!;
    }
}
