using System.Security.Claims;
using SharpSite.Abstractions.Security;
using Microsoft.AspNetCore.Identity;

namespace SharpSite.Security.Postgres;

/// <summary>
/// Implementation of IUserManager for PostgreSQL using ASP.NET Core Identity
/// </summary>
public class PgUserManager : IUserManager<ISharpSiteUser>
{
    private readonly UserManager<PgSharpSiteUser> _userManager;

    public PgUserManager(UserManager<PgSharpSiteUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<string> GetUserIdAsync(ISharpSiteUser user)
    {
        var pgUser = (PgSharpSiteUser)user;
        return await _userManager.GetUserIdAsync(pgUser);
    }

    public async Task<string?> GetUserNameAsync(ISharpSiteUser user)
    {
        var pgUser = (PgSharpSiteUser)user;
        return await _userManager.GetUserNameAsync(pgUser);
    }

    public async Task<bool> HasPasswordAsync(ISharpSiteUser user)
    {
        var pgUser = (PgSharpSiteUser)user;
        return await _userManager.HasPasswordAsync(pgUser);
    }

    public async Task<ISharpSiteUser?> GetUserAsync(ClaimsPrincipal principal)
    {
        var pgUser = await _userManager.GetUserAsync(principal);
        return pgUser is null ? null : (ISharpSiteUser)pgUser;
    }

    public async Task<IdentityResult> CreateAsync(ISharpSiteUser user, string password)
    {
        var pgUser = (PgSharpSiteUser)user;
        return await _userManager.CreateAsync(pgUser, password);
    }

    public async Task<IdentityResult> AddToRoleAsync(ISharpSiteUser user, string role)
    {
        var pgUser = (PgSharpSiteUser)user;
        return await _userManager.AddToRoleAsync(pgUser, role);
    }

    public async Task<IdentityResult> RemoveFromRoleAsync(ISharpSiteUser user, string role)
    {
        var pgUser = (PgSharpSiteUser)user;
        return await _userManager.RemoveFromRoleAsync(pgUser, role);
    }

    public async Task<IList<string>> GetRolesAsync(ISharpSiteUser user)
    {
        var pgUser = (PgSharpSiteUser)user;
        return await _userManager.GetRolesAsync(pgUser);
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(ISharpSiteUser user)
    {
        var pgUser = (PgSharpSiteUser)user;
        return await _userManager.GenerateEmailConfirmationTokenAsync(pgUser);
    }

    public async Task<bool> GetTwoFactorEnabledAsync(ISharpSiteUser user)
    {
        var pgUser = (PgSharpSiteUser)user;
        return await _userManager.GetTwoFactorEnabledAsync(pgUser);
    }

    public async Task<string> GetAuthenticatorKeyAsync(ISharpSiteUser user)
    {
        var pgUser = (PgSharpSiteUser)user;
        return await _userManager.GetAuthenticatorKeyAsync(pgUser);
    }

    public async Task<IdentityResult> SetTwoFactorEnabledAsync(ISharpSiteUser user, bool enabled)
    {
        var pgUser = (PgSharpSiteUser)user;
        return await _userManager.SetTwoFactorEnabledAsync(pgUser, enabled);
    }

    public async Task<IdentityResult> ResetAuthenticatorKeyAsync(ISharpSiteUser user)
    {
        var pgUser = (PgSharpSiteUser)user;
        return await _userManager.ResetAuthenticatorKeyAsync(pgUser);
    }

    public async Task<IEnumerable<ISharpSiteUser>> GetUsersInRoleAsync(string role)
    {
        var pgUsers = await _userManager.GetUsersInRoleAsync(role);
        return pgUsers.Select(u => (ISharpSiteUser)u);
    }

    public async Task<bool> VerifyTwoFactorTokenAsync(ISharpSiteUser user, string tokenProvider, string token)
    {
        var pgUser = (PgSharpSiteUser)user;
        return await _userManager.VerifyTwoFactorTokenAsync(pgUser, tokenProvider, token);
    }

    public async Task<int> CountRecoveryCodesAsync(ISharpSiteUser user)
    {
        var pgUser = (PgSharpSiteUser)user;
        return await _userManager.CountRecoveryCodesAsync(pgUser);
    }

    public async Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryCodesAsync(ISharpSiteUser user, int number)
    {
        var pgUser = (PgSharpSiteUser)user;
        return await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(pgUser, number);
    }

    public async Task<IdentityResult> UpdateAsync(ISharpSiteUser user)
    {
        var pgUser = (PgSharpSiteUser)user;
        return await _userManager.UpdateAsync(pgUser);
    }

    public async Task<IdentityResult> DeleteAsync(ISharpSiteUser user)
    {
        var pgUser = (PgSharpSiteUser)user;
        return await _userManager.DeleteAsync(pgUser);
    }

    public async Task<bool> CheckPasswordAsync(ISharpSiteUser user, string password)
    {
        var pgUser = (PgSharpSiteUser)user;
        return await _userManager.CheckPasswordAsync(pgUser, password);
    }
}
