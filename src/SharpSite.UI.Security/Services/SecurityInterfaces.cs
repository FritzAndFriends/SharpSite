using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace SharpSite.UI.Security.Services;

public interface ISignInManager
{
    Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();
    Task<SignInResult> PasswordSignInAsync(string userName, string password, bool rememberMe, bool lockoutOnFailure);
    Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor);
    Task RefreshSignInAsync(ISharpSiteUser user);
    AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, string? userId = null);
    Task SignOutAsync();
    Task SignInAsync(ISharpSiteUser user, bool isPersistent, string? authenticationMethod = null);
    Task<ExternalLoginInfo?> GetExternalLoginInfoAsync(string? expectedXsrf = null);
    Task<bool> IsTwoFactorClientRememberedAsync(ISharpSiteUser user);
    Task ForgetTwoFactorClientAsync();
}

public interface ISharpSiteUser
{
    string Id { get; set; }
    string UserName { get; set; }
    string Email { get; set; }
    string? PhoneNumber { get; set; }
    bool PhoneNumberConfirmed { get; set; }
    bool TwoFactorEnabled { get; set; }
    bool EmailConfirmed { get; set; }
    string? DisplayName { get; set; }
}

public interface IUserManager
{
    Task<ISharpSiteUser?> GetUserAsync(ClaimsPrincipal user);
    Task<string> GetUserNameAsync(ISharpSiteUser user);
    Task<string?> GetPhoneNumberAsync(ISharpSiteUser user);
    Task<IdentityResult> SetPhoneNumberAsync(ISharpSiteUser user, string? phoneNumber);
    Task<ISharpSiteUser?> FindByIdAsync(string userId);
    Task<IdentityResult> UpdateAsync(ISharpSiteUser user);
    Task<bool> IsEmailConfirmedAsync(ISharpSiteUser user);
    Task<string> GenerateEmailConfirmationTokenAsync(ISharpSiteUser user);
    Task<IdentityResult> ConfirmEmailAsync(ISharpSiteUser user, string token);
    Task<string> GetEmailAsync(ISharpSiteUser user);
    Task<IdentityResult> SetEmailAsync(ISharpSiteUser user, string email);
    Task<string?> GetAuthenticatorKeyAsync(ISharpSiteUser user);
    Task<bool> VerifyTwoFactorTokenAsync(ISharpSiteUser user, string tokenProvider, string token);
    Task<IList<string>> GenerateNewTwoFactorRecoveryCodesAsync(ISharpSiteUser user, int number);
    Task<bool> IsTwoFactorEnabledAsync(ISharpSiteUser user);
    Task<IdentityResult> SetTwoFactorEnabledAsync(ISharpSiteUser user, bool enabled);
    Task<IdentityResult> ResetAuthenticatorKeyAsync(ISharpSiteUser user);
    Task<int> CountRecoveryCodesAsync(ISharpSiteUser user);
    Task<IList<UserLoginInfo>> GetLoginsAsync(ISharpSiteUser user);
    Task<IdentityResult> RemoveLoginAsync(ISharpSiteUser user, string loginProvider, string providerKey);
    Task<IdentityResult> AddLoginAsync(ISharpSiteUser user, UserLoginInfo info);
    Task<ISharpSiteUser?> FindByEmailAsync(string email);
    Task<bool> HasPasswordAsync(ISharpSiteUser user);
    Task<IdentityResult> AddPasswordAsync(ISharpSiteUser user, string password);
    Task<IdentityResult> ChangePasswordAsync(ISharpSiteUser user, string oldPassword, string newPassword);
    Task<string> GetUserIdAsync(ISharpSiteUser user);
    Task<IdentityResult> CreateAsync(ISharpSiteUser user);
    Task<IdentityResult> CreateAsync(ISharpSiteUser user, string password);
    Task<bool> GetTwoFactorEnabledAsync(ISharpSiteUser user);
    Task<string> GenerateChangeEmailTokenAsync(ISharpSiteUser user, string newEmail);
    Task<bool> CheckPasswordAsync(ISharpSiteUser user, string password);
    Task<IdentityResult> DeleteAsync(ISharpSiteUser user);
    IdentityOptions Options { get; }
}
