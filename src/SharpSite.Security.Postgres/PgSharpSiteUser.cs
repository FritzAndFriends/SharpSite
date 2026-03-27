using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharpSite.Abstractions;
using AbsSecurity = SharpSite.Abstractions.Security;
using System.ComponentModel.DataAnnotations;

namespace SharpSite.Security.Postgres;

public class PgSharpSiteUser : IdentityUser, AbsSecurity.ISharpSiteUser
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

    public static PgSharpSiteUser FromInterface(AbsSecurity.ISharpSiteUser user) =>
        user as PgSharpSiteUser ?? new()
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

public class PgSecurityContext : IdentityDbContext<PgSharpSiteUser>
{
    public PgSecurityContext(DbContextOptions<PgSecurityContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Claim and Roles are in-memory convenience properties, not EF-mapped columns.
        // EF Core 10 attempts to bind System.Security.Claims.Claim as an owned type,
        // but Claim has no constructor EF can use — so we must exclude them.
        builder.Entity<PgSharpSiteUser>().Ignore(u => u.Claims);
        builder.Entity<PgSharpSiteUser>().Ignore(u => u.Roles);
    }
}