using AbsSecurity = SharpSite.Abstractions.Security;

namespace SharpSite.Plugins.Data.Postgres.Security;

public class LoginInfo(string loginProvider, string providerKey, string displayName) : AbsSecurity.ILoginInfo
{
	public string LoginProvider { get; set; } = loginProvider;
	public string ProviderKey { get; set; } = providerKey;
	public string ProviderDisplayName { get; set; } = displayName;
}