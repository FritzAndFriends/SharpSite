using SharpSite.Abstractions.Base;
using SharpSite.Abstractions.Security;
using SSS = SharpSite.Abstractions.Security;

namespace SharpSite.Plugins.Data.Postgres.Security;

[RegisterPlugin(PluginServiceLocatorScope.Scoped, PluginRegisterType.Security_SignInManager)]

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

	public async Task<SSS.SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
	{
		return (await _signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure)).ToSharpSiteSignInResult();
	}

	public async Task<bool> IsTwoFactorClientRememberedAsync(ISharpSiteUser user)
	{
		var pgUser = (PgSharpSiteUser)user;
		return await _signInManager.IsTwoFactorClientRememberedAsync(pgUser);
	}

	public async Task<SSS.SignInResult> TwoFactorAuthenticatorSignInAsync(string code, bool isPersistent, bool rememberClient)
	{
		return (await _signInManager.TwoFactorAuthenticatorSignInAsync(code, isPersistent, rememberClient)).ToSharpSiteSignInResult();
	}

	public async Task<ISharpSiteUser?> GetTwoFactorAuthenticationUserAsync()
	{
		var pgUser = await _signInManager.GetTwoFactorAuthenticationUserAsync();
		return pgUser is null ? null : (ISharpSiteUser)pgUser;
	}

	public async Task<IEnumerable<SSS.AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
	{
		return (await _signInManager.GetExternalAuthenticationSchemesAsync()).Select(scheme => scheme.ToSharpSiteAuthenticationScheme());
	}

	public async Task ForgetTwoFactorClientAsync()
	{
		await _signInManager.ForgetTwoFactorClientAsync();
	}

	public async Task<ILoginInfo?> GetExternalLoginInfoAsync(string expectedXsrf = null!)
	{
		return (await _signInManager.GetExternalLoginInfoAsync(expectedXsrf)).ToSharpSiteLoginInfo();
	}

	public async Task<SSS.SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent)
	{
		return (await _signInManager.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent)).ToSharpSiteSignInResult();
	}

	public async Task RefreshSignInAsync(ISharpSiteUser user)
	{
		var pgUser = (PgSharpSiteUser)user;
		await _signInManager.RefreshSignInAsync(pgUser);
	}

}


public static class SignInResultExtensions
{
	public static SSS.SignInResult ToSharpSiteSignInResult(this Microsoft.AspNetCore.Identity.SignInResult result)
	{
		return new SSS.SignInResult(
				result.Succeeded,
				result.IsLockedOut,
				result.IsNotAllowed,
				result.RequiresTwoFactor);
	}

	// need a static method to convert AuthenicationScheme to SharpSite.AuthenticationScheme
	public static SSS.AuthenticationScheme ToSharpSiteAuthenticationScheme(this Microsoft.AspNetCore.Authentication.AuthenticationScheme scheme)
	{
		return new SSS.AuthenticationScheme(
				scheme.Name,
				scheme.DisplayName ?? string.Empty,
				scheme.HandlerType.AssemblyQualifiedName ?? string.Empty
		);
	}

	// static method to convert ExtertnalLoginInfo to LoginInfo
	public static ILoginInfo ToSharpSiteLoginInfo(this ExternalLoginInfo? info)
	{

		if (info == null)
		{
			// if the info is null, return an empty LoginInfo
			return new LoginInfo(string.Empty, string.Empty, string.Empty);
		}

		return new LoginInfo(
			info.LoginProvider,
			info.ProviderKey,
			info.ProviderDisplayName ?? string.Empty
		);
	}


}
