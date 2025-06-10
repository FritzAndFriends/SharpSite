using System.Security.Claims;
using SharpSite.Abstractions.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;

namespace SharpSite.Security.Postgres;

/// <summary>
/// Implementation of ISignInManager for PostgreSQL using ASP.NET Core Identity
/// </summary>
public class PgSignInManager : ISignInManager<ISharpSiteUser>
{
    private readonly SignInManager<PgSharpSiteUser> _signInManager;

    public PgSignInManager(SignInManager<PgSharpSiteUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
    {
        return await _signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
    }

    public async Task<bool> IsTwoFactorClientRememberedAsync(ISharpSiteUser user)
    {
        var pgUser = (PgSharpSiteUser)user;
        return await _signInManager.IsTwoFactorClientRememberedAsync(pgUser);
    }

    public async Task<SignInResult> TwoFactorAuthenticatorSignInAsync(string code, bool isPersistent, bool rememberClient)
    {
        return await _signInManager.TwoFactorAuthenticatorSignInAsync(code, isPersistent, rememberClient);
    }

    public async Task<ISharpSiteUser?> GetTwoFactorAuthenticationUserAsync()
    {
        var pgUser = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        return pgUser is null ? null : (ISharpSiteUser)pgUser;
    }

    public async Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
    {
        return await _signInManager.GetExternalAuthenticationSchemesAsync();
    }

    public async Task ForgetTwoFactorClientAsync()
    {
        await _signInManager.ForgetTwoFactorClientAsync();
    }

    public async Task<IUserLoginInfo?> GetExternalLoginInfoAsync(string expectedXsrf = null!)
    {
        var loginInfo = await _signInManager.GetExternalLoginInfoAsync(expectedXsrf);
        return loginInfo;
    }

    public async Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent) 
    {
        return await _signInManager.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent);
    }

    public async Task RefreshSignInAsync(ISharpSiteUser user)
    {
        var pgUser = (PgSharpSiteUser)user;
        await _signInManager.RefreshSignInAsync(pgUser);
    }
}
