using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SharpSite.Abstractions.Base;
using SharpSite.Abstractions.Security;
using SSS = SharpSite.Abstractions.Security;

namespace SharpSite.Plugins.Data.Postgres.Security;

[RegisterPlugin(PluginServiceLocatorScope.Scoped, PluginRegisterType.Security_UserManager)]
public class PgUserManager : IUserManager
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

	public async Task<SSS.IdentityResult> CreateAsync(ISharpSiteUser user, string password)
	{
		var pgUser = (PgSharpSiteUser)user;
		return (await _userManager.CreateAsync(pgUser, password)).ToSharpSiteIdentityResult();
	}

	public async Task<SSS.IdentityResult> AddToRoleAsync(ISharpSiteUser user, string role)
	{
		var pgUser = (PgSharpSiteUser)user;
		return (await _userManager.AddToRoleAsync(pgUser, role)).ToSharpSiteIdentityResult();
	}

	public async Task<SSS.IdentityResult> RemoveFromRoleAsync(ISharpSiteUser user, string role)
	{
		var pgUser = (PgSharpSiteUser)user;
		return (await _userManager.RemoveFromRoleAsync(pgUser, role)).ToSharpSiteIdentityResult();
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
		return await _userManager.GetAuthenticatorKeyAsync(pgUser) ?? string.Empty;
	}

	public async Task<SSS.IdentityResult> SetTwoFactorEnabledAsync(ISharpSiteUser user, bool enabled)
	{
		var pgUser = (PgSharpSiteUser)user;
		return (await _userManager.SetTwoFactorEnabledAsync(pgUser, enabled)).ToSharpSiteIdentityResult();
	}

	public async Task<SSS.IdentityResult> ResetAuthenticatorKeyAsync(ISharpSiteUser user)
	{
		var pgUser = (PgSharpSiteUser)user;
		return (await _userManager.ResetAuthenticatorKeyAsync(pgUser)).ToSharpSiteIdentityResult();
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
		return await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(pgUser, number) ?? Enumerable.Empty<string>();
	}

	public async Task<SSS.IdentityResult> UpdateAsync(ISharpSiteUser user)
	{
		var pgUser = (PgSharpSiteUser)user;
		return (await _userManager.UpdateAsync(pgUser)).ToSharpSiteIdentityResult();
	}

	public async Task<SSS.IdentityResult> DeleteAsync(ISharpSiteUser user)
	{
		var pgUser = (PgSharpSiteUser)user;
		return (await _userManager.DeleteAsync(pgUser)).ToSharpSiteIdentityResult();
	}

	public async Task<bool> CheckPasswordAsync(ISharpSiteUser user, string password)
	{
		var pgUser = (PgSharpSiteUser)user;
		return await _userManager.CheckPasswordAsync(pgUser, password);
	}
}

public static class UserManagerExtensions
{
		public static SSS.IdentityResult ToSharpSiteIdentityResult(this Microsoft.AspNetCore.Identity.IdentityResult result)
		{
				return new SSS.IdentityResult(result.Succeeded, result.Errors.Select(e => new SSS.IdentityError
				{
						Code = e.Code,
						Description = e.Description
				}));
		}
}