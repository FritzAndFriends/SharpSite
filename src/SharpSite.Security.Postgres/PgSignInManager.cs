using System.Security.Claims;
using AbsSecurity = SharpSite.Abstractions.Security;
using MsIdentity = Microsoft.AspNetCore.Identity;
using MsAuth = Microsoft.AspNetCore.Authentication;

namespace SharpSite.Security.Postgres;

/// <summary>
/// Implementation of ISignInManager for PostgreSQL using ASP.NET Core Identity
/// </summary>
public class PgSignInManager : AbsSecurity.ISignInManager
{
    private readonly MsIdentity.SignInManager<PgSharpSiteUser> _signInManager;

    public PgSignInManager(MsIdentity.SignInManager<PgSharpSiteUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<AbsSecurity.SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
    {
        var result = await _signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
        return ToSignInResult(result);
    }

    public async Task<bool> IsTwoFactorClientRememberedAsync(AbsSecurity.ISharpSiteUser user)
    {
        var pgUser = PgSharpSiteUser.FromInterface(user);
        return await _signInManager.IsTwoFactorClientRememberedAsync(pgUser);
    }

    public async Task<AbsSecurity.SignInResult> TwoFactorAuthenticatorSignInAsync(string code, bool isPersistent, bool rememberClient)
    {
        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(code, isPersistent, rememberClient);
        return ToSignInResult(result);
    }

    public async Task<AbsSecurity.ISharpSiteUser?> GetTwoFactorAuthenticationUserAsync()
    {
        var pgUser = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        return pgUser;
    }

    public async Task<IEnumerable<AbsSecurity.AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
    {
        var schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
        return schemes.Select(s => new AbsSecurity.AuthenticationScheme(s.Name, s.DisplayName ?? s.Name, s.HandlerType.FullName ?? s.HandlerType.Name));
    }

    public async Task ForgetTwoFactorClientAsync()
    {
        await _signInManager.ForgetTwoFactorClientAsync();
    }

    public async Task<AbsSecurity.ILoginInfo?> GetExternalLoginInfoAsync(string expectedXsrf = null!)
    {
        var loginInfo = await _signInManager.GetExternalLoginInfoAsync(expectedXsrf);
        if (loginInfo is null) return null;
        return new ExternalLoginInfoAdapter(loginInfo.LoginProvider, loginInfo.ProviderKey, loginInfo.ProviderDisplayName ?? loginInfo.LoginProvider);
    }

    public async Task<AbsSecurity.SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent)
    {
        var result = await _signInManager.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent);
        return ToSignInResult(result);
    }

    public async Task RefreshSignInAsync(AbsSecurity.ISharpSiteUser user)
    {
        var pgUser = PgSharpSiteUser.FromInterface(user);
        await _signInManager.RefreshSignInAsync(pgUser);
    }

    private static AbsSecurity.SignInResult ToSignInResult(MsIdentity.SignInResult result) =>
        new(result.Succeeded, result.IsLockedOut, result.IsNotAllowed, result.RequiresTwoFactor);
}

internal sealed class ExternalLoginInfoAdapter(string loginProvider, string providerKey, string providerDisplayName) : AbsSecurity.ILoginInfo
{
    public string LoginProvider => loginProvider;
    public string ProviderKey => providerKey;
    public string ProviderDisplayName => providerDisplayName;
}
