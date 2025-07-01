using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace SharpSite.Abstractions.Security;

/// <summary>
/// Provider-agnostic user management interface
/// </summary>
public interface IUserManager
{
    Task<string> GetUserIdAsync(ISharpSiteUser user);
    Task<string?> GetUserNameAsync(ISharpSiteUser user);
    Task<bool> HasPasswordAsync(ISharpSiteUser user);
    Task<ISharpSiteUser?> GetUserAsync(ClaimsPrincipal principal);
    Task<IdentityResult> CreateAsync(ISharpSiteUser user, string password);
    Task<IdentityResult> AddToRoleAsync(ISharpSiteUser user, string role);
    Task<IdentityResult> RemoveFromRoleAsync(ISharpSiteUser user, string role);
    Task<IList<string>> GetRolesAsync(ISharpSiteUser user);
    Task<string> GenerateEmailConfirmationTokenAsync(ISharpSiteUser user);
    Task<bool> GetTwoFactorEnabledAsync(ISharpSiteUser user);
    Task<string> GetAuthenticatorKeyAsync(ISharpSiteUser user);
    Task<IdentityResult> SetTwoFactorEnabledAsync(ISharpSiteUser user, bool enabled);
    Task<IdentityResult> ResetAuthenticatorKeyAsync(ISharpSiteUser user);
    Task<IEnumerable<ISharpSiteUser>> GetUsersInRoleAsync(string role);
    Task<bool> VerifyTwoFactorTokenAsync(ISharpSiteUser user, string tokenProvider, string token);
    Task<int> CountRecoveryCodesAsync(ISharpSiteUser user);
    Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryCodesAsync(ISharpSiteUser user, int number);
    Task<IdentityResult> UpdateAsync(ISharpSiteUser user);
    Task<IdentityResult> DeleteAsync(ISharpSiteUser user);
    Task<bool> CheckPasswordAsync(ISharpSiteUser user, string password);
}

/// <summary>
/// Provider-agnostic sign-in management interface
/// </summary>
public interface ISignInManager<TUser> where TUser : class
{
    Task SignOutAsync();
    Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure);
    Task<bool> IsTwoFactorClientRememberedAsync(TUser user);
    Task<SignInResult> TwoFactorAuthenticatorSignInAsync(string code, bool isPersistent, bool rememberClient);
    Task<TUser?> GetTwoFactorAuthenticationUserAsync();
    Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();
    Task ForgetTwoFactorClientAsync();
    Task<ILoginInfo?> GetExternalLoginInfoAsync(string expectedXsrf = null!);
    Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent);
    Task RefreshSignInAsync(TUser user);
}

/// <summary>
/// Provider-agnostic email management interface
/// </summary>
public interface IEmailSender<TUser> where TUser : class
{
    Task SendConfirmationLinkAsync(TUser user, string email, string confirmationLink);
    Task SendPasswordResetLinkAsync(TUser user, string email, string resetLink);
    Task SendPasswordResetCodeAsync(TUser user, string email, string resetCode);
}
