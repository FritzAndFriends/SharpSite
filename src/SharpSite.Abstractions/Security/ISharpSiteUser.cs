using System.Security.Claims;

namespace SharpSite.Abstractions.Security; 

/// <summary>
/// Provider-agnostic interface representing a user in the system
/// </summary>
public interface ISharpSiteUser
{
    string Id { get; }
    string? UserName { get; set; }
    string? NormalizedUserName { get; set; }
    string? Email { get; set; }
    string? NormalizedEmail { get; set; }
    bool EmailConfirmed { get; set; }
    string? PhoneNumber { get; set; }
    bool PhoneNumberConfirmed { get; set; }
    bool TwoFactorEnabled { get; set; }
    DateTimeOffset? LockoutEnd { get; set; }
    bool LockoutEnabled { get; set; }
    int AccessFailedCount { get; set; }
    string? SecurityStamp { get; set; }
    string? ConcurrencyStamp { get; set; }
    string? PasswordHash { get; set; }
    string DisplayName { get; set; }
    IList<string> Roles { get; }
    IList<Claim> Claims { get; }
}
