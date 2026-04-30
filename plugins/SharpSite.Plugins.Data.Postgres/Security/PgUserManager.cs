using System.Security.Claims;
using SharpSite.Abstractions.Base;
using AbsSecurity = SharpSite.Abstractions.Security;
using MsIdentity = Microsoft.AspNetCore.Identity;

namespace SharpSite.Plugins.Data.Postgres.Security;

[RegisterPlugin(PluginServiceLocatorScope.Scoped, PluginRegisterType.Security_UserManager)]
public class PgUserManager : AbsSecurity.IUserManager
{
	private readonly MsIdentity.UserManager<PgSharpSiteUser> _userManager;

	public PgUserManager(MsIdentity.UserManager<PgSharpSiteUser> userManager)
	{
		_userManager = userManager;
	}

	public async Task<string> GetUserIdAsync(AbsSecurity.ISharpSiteUser user)
	{
		var pgUser = PgSharpSiteUser.FromInterface(user);
		return await _userManager.GetUserIdAsync(pgUser);
	}

	public async Task<string?> GetUserNameAsync(AbsSecurity.ISharpSiteUser user)
	{
		var pgUser = PgSharpSiteUser.FromInterface(user);
		return await _userManager.GetUserNameAsync(pgUser);
	}

	public async Task<bool> HasPasswordAsync(AbsSecurity.ISharpSiteUser user)
	{
		var pgUser = PgSharpSiteUser.FromInterface(user);
		return await _userManager.HasPasswordAsync(pgUser);
	}

	public async Task<AbsSecurity.ISharpSiteUser?> GetUserAsync(ClaimsPrincipal principal)
	{
		var pgUser = await _userManager.GetUserAsync(principal);
		return pgUser;
	}

	public async Task<AbsSecurity.IdentityResult> CreateAsync(AbsSecurity.ISharpSiteUser user, string password)
	{
		var pgUser = PgSharpSiteUser.FromInterface(user);
		return ToIdentityResult(await _userManager.CreateAsync(pgUser, password));
	}

	public async Task<AbsSecurity.IdentityResult> AddToRoleAsync(AbsSecurity.ISharpSiteUser user, string role)
	{
		var pgUser = PgSharpSiteUser.FromInterface(user);
		return ToIdentityResult(await _userManager.AddToRoleAsync(pgUser, role));
	}

	public async Task<AbsSecurity.IdentityResult> RemoveFromRoleAsync(AbsSecurity.ISharpSiteUser user, string role)
	{
		var pgUser = PgSharpSiteUser.FromInterface(user);
		return ToIdentityResult(await _userManager.RemoveFromRoleAsync(pgUser, role));
	}

	public async Task<IList<string>> GetRolesAsync(AbsSecurity.ISharpSiteUser user)
	{
		var pgUser = PgSharpSiteUser.FromInterface(user);
		return await _userManager.GetRolesAsync(pgUser);
	}

	public async Task<string> GenerateEmailConfirmationTokenAsync(AbsSecurity.ISharpSiteUser user)
	{
		var pgUser = PgSharpSiteUser.FromInterface(user);
		return await _userManager.GenerateEmailConfirmationTokenAsync(pgUser);
	}

	public async Task<bool> GetTwoFactorEnabledAsync(AbsSecurity.ISharpSiteUser user)
	{
		var pgUser = PgSharpSiteUser.FromInterface(user);
		return await _userManager.GetTwoFactorEnabledAsync(pgUser);
	}

	public async Task<string> GetAuthenticatorKeyAsync(AbsSecurity.ISharpSiteUser user)
	{
		var pgUser = PgSharpSiteUser.FromInterface(user);
		return await _userManager.GetAuthenticatorKeyAsync(pgUser) ?? string.Empty;
	}

	public async Task<AbsSecurity.IdentityResult> SetTwoFactorEnabledAsync(AbsSecurity.ISharpSiteUser user, bool enabled)
	{
		var pgUser = PgSharpSiteUser.FromInterface(user);
		return ToIdentityResult(await _userManager.SetTwoFactorEnabledAsync(pgUser, enabled));
	}

	public async Task<AbsSecurity.IdentityResult> ResetAuthenticatorKeyAsync(AbsSecurity.ISharpSiteUser user)
	{
		var pgUser = PgSharpSiteUser.FromInterface(user);
		return ToIdentityResult(await _userManager.ResetAuthenticatorKeyAsync(pgUser));
	}

	public async Task<IEnumerable<AbsSecurity.ISharpSiteUser>> GetUsersInRoleAsync(string role)
	{
		var pgUsers = await _userManager.GetUsersInRoleAsync(role);
		return pgUsers.Cast<AbsSecurity.ISharpSiteUser>();
	}

	public async Task<bool> VerifyTwoFactorTokenAsync(AbsSecurity.ISharpSiteUser user, string tokenProvider, string token)
	{
		var pgUser = PgSharpSiteUser.FromInterface(user);
		return await _userManager.VerifyTwoFactorTokenAsync(pgUser, tokenProvider, token);
	}

	public async Task<int> CountRecoveryCodesAsync(AbsSecurity.ISharpSiteUser user)
	{
		var pgUser = PgSharpSiteUser.FromInterface(user);
		return await _userManager.CountRecoveryCodesAsync(pgUser);
	}

	public async Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryCodesAsync(AbsSecurity.ISharpSiteUser user, int number)
	{
		var pgUser = PgSharpSiteUser.FromInterface(user);
		return await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(pgUser, number) ?? Enumerable.Empty<string>();
	}

	public async Task<AbsSecurity.IdentityResult> UpdateAsync(AbsSecurity.ISharpSiteUser user)
	{
		var pgUser = PgSharpSiteUser.FromInterface(user);
		return ToIdentityResult(await _userManager.UpdateAsync(pgUser));
	}

	public async Task<AbsSecurity.IdentityResult> DeleteAsync(AbsSecurity.ISharpSiteUser user)
	{
		var pgUser = PgSharpSiteUser.FromInterface(user);
		return ToIdentityResult(await _userManager.DeleteAsync(pgUser));
	}

	public async Task<bool> CheckPasswordAsync(AbsSecurity.ISharpSiteUser user, string password)
	{
		var pgUser = PgSharpSiteUser.FromInterface(user);
		return await _userManager.CheckPasswordAsync(pgUser, password);
	}

	public string GetUserId(System.Security.Claims.ClaimsPrincipal principal)
	{
		return _userManager.GetUserId(principal) ?? string.Empty;
	}

	public async Task<string> GenerateChangeEmailTokenAsync(AbsSecurity.ISharpSiteUser user, string newEmail)
	{
		var pgUser = PgSharpSiteUser.FromInterface(user);
		return await _userManager.GenerateChangeEmailTokenAsync(pgUser, newEmail);
	}

	public MsIdentity.IdentityOptions Options => _userManager.Options;

	private static AbsSecurity.IdentityResult ToIdentityResult(MsIdentity.IdentityResult result) =>
		new(result.Succeeded, result.Errors.Select(e => new AbsSecurity.IdentityError { Code = e.Code, Description = e.Description }));
}