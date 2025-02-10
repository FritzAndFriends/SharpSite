using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SharpSite.Data.Postgres;

public class PgContext : DbContext
{

	public PgContext(DbContextOptions<PgContext> options) : base(options) { }

	public DbSet<PgPage> Pages => Set<PgPage>();

	public DbSet<PgPost> Posts => Set<PgPost>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{

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
