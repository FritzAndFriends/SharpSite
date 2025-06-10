using Microsoft.AspNetCore.Identity;

namespace SharpSite.Abstractions.Security;

/// <summary>
/// Provider-agnostic login information.
/// </summary>
public interface IUserLoginInfo
{
    string LoginProvider { get; }
    string ProviderKey { get; }
    string ProviderDisplayName { get; }
}
