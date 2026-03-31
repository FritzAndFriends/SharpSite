using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SharpSite.Plugins.Data.Postgres;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PgContext>
{
    public PgContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PgContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=sharpsite;Username=sharpsite;Password=sharpsite");

        return new PgContext(optionsBuilder.Options);
    }
}
