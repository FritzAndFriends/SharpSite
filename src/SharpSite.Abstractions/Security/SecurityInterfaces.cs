using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace SharpSite.Abstractions.Security;

/// <summary>
/// Provider-agnostic user management interface
/// </summary>
public interface IUserManager<TUser> where TUser : class
{
    Task<string> GetUserIdAsync(TUser user);
    Task<string?> GetUserNameAsync(TUser user);
    Task<bool> HasPasswordAsync(TUser user);
    Task<TUser?> GetUserAsync(ClaimsPrincipal principal);
    Task<IdentityResult> CreateAsync(TUser user, string password);
    Task<IdentityResult> AddToRoleAsync(TUser user, string role);
    Task<IdentityResult> RemoveFromRoleAsync(TUser user, string role);
    Task<IList<string>> GetRolesAsync(TUser user);
    Task<string> GenerateEmailConfirmationTokenAsync(TUser user);
    Task<bool> GetTwoFactorEnabledAsync(TUser user);
    Task<string> GetAuthenticatorKeyAsync(TUser user);
    Task<IdentityResult> SetTwoFactorEnabledAsync(TUser user, bool enabled);
    Task<IdentityResult> ResetAuthenticatorKeyAsync(TUser user);
    Task<IEnumerable<TUser>> GetUsersInRoleAsync(string role);
    Task<bool> VerifyTwoFactorTokenAsync(TUser user, string tokenProvider, string token);
    Task<int> CountRecoveryCodesAsync(TUser user);
    Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryCodesAsync(TUser user, int number);
    Task<IdentityResult> UpdateAsync(TUser user);
    Task<IdentityResult> DeleteAsync(TUser user);
    Task<bool> CheckPasswordAsync(TUser user, string password);
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
