using SharpSite.Abstractions.Security;

namespace SharpSite.Plugins.Data.Postgres.Security;

public class LoginInfo(string loginProvider, string providerKey, string displayName) : ILoginInfo
{
	public string LoginProvider { get; set; } = loginProvider;
	public string ProviderKey { get; set; } = providerKey;
	public string ProviderDisplayName { get; set; } = displayName;
}