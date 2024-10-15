using Microsoft.EntityFrameworkCore;

namespace SharpSite.Data.Postgres;

public class PgContext : DbContext
{

	public PgContext(DbContextOptions<PgContext> options) : base(options) { }

	public DbSet<PgPost> Posts { get; set; }
	
}
