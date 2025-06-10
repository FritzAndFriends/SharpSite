using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations;
using SharpSite.Abstractions.Base;
using SharpSite.Plugins.Data.Postgres.Security;

namespace SharpSite.Plugins.Data.Postgres;

[RegisterPlugin(PluginServiceLocatorScope.Singleton, PluginRegisterType.DataStorage_EfContext)]
public class PgContext : IdentityDbContext<PgSharpSiteUser>
{
    public PgContext(DbContextOptions<PgContext> options) : base(options) { }

    public DbSet<PgPage> Pages => Set<PgPage>();
    public DbSet<PgPost> Posts => Set<PgPost>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Important for Identity tables

        modelBuilder.Entity<PgPage>()
            .HasIndex(p => p.Slug)
            .IsUnique();

        modelBuilder
            .Entity<PgPost>()
            .Property(e => e.Published)
            .HasConversion(new DateTimeOffsetConverter());

        modelBuilder
            .Entity<PgPost>()
            .Property(e => e.LastUpdate)
            .HasConversion(new DateTimeOffsetConverter());

        modelBuilder
            .Entity<PgPage>()
            .Property(e => e.LastUpdate)
            .HasConversion(new DateTimeOffsetConverter());
    }
}

public class DateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
{
    public DateTimeOffsetConverter() : base(
            v => v.UtcDateTime,
            v => v)
    {
    }
}
