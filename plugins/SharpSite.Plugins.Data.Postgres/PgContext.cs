using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SharpSite.Abstractions.Base;

namespace SharpSite.Plugins.Data.Postgres;

[RegisterPlugin(PluginServiceLocatorScope.Transient, PluginRegisterType.DataStorage_EfContext)]
public class PgContext : DbContext
{

	private readonly string? _ConnectionString;

	public PgContext(DbContextOptions<PgContext> options) : base(options) { }

	// add a default configuration for this context that uses Postgres and gets the connection string from ApplicationState
	public PgContext(IApplicationStateModel appState)
	{
		_ConnectionString = appState.GetConfigurationByName(ApplicationStateKeys.ContentConnectionString);
	}

	public DbSet<PgPage> Pages => Set<PgPage>();

	public DbSet<PgPost> Posts => Set<PgPost>();

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{

		// configure to use the Npgsql database and with _Connectionstring configured
		if (_ConnectionString != null)
		{
			optionsBuilder.UseNpgsql(_ConnectionString);
		}

		base.OnConfiguring(optionsBuilder);
	}

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
