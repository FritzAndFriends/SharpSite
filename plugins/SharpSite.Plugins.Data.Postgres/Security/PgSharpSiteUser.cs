using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SharpSite.Abstractions;
using SharpSite.Abstractions.Security;
using System.ComponentModel.DataAnnotations;

namespace SharpSite.Plugins.Data.Postgres.Security;

public class PgSharpSiteUser : IdentityUser, ISharpSiteUser
{
    [PersonalData, Required, MaxLength(50)]
    public required string DisplayName { get; set; }

    // Roles and Claims properties to fulfill ISharpSiteUser interface
    public IList<string> Roles { get; } = new List<string>();
    public IList<Claim> Claims { get; } = new List<Claim>();

    public static explicit operator SharpSiteUser(PgSharpSiteUser user) =>
        new(user.Id, user.UserName, user.Email)
        {
            DisplayName = user.DisplayName,
            PhoneNumber = user.PhoneNumber
        };

    public static explicit operator PgSharpSiteUser(SharpSiteUser user) =>
        new()
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

    public static explicit operator ISharpSiteUser(PgSharpSiteUser user) => user;

    public static explicit operator PgSharpSiteUser(ISharpSiteUser user) =>
        new()
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            TwoFactorEnabled = user.TwoFactorEnabled,
            LockoutEnd = user.LockoutEnd,
            LockoutEnabled = user.LockoutEnabled,
            AccessFailedCount = user.AccessFailedCount,
            SecurityStamp = user.SecurityStamp,
            ConcurrencyStamp = user.ConcurrencyStamp,
            PasswordHash = user.PasswordHash
        };
}
