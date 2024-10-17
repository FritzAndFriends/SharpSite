using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OpenTelemetry.Trace;
using Microsoft.EntityFrameworkCore;

namespace SharpSite.Data.Postgres.Migration;

public class Worker(
		IServiceProvider serviceProvider,
		IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
	public const string ActivitySourceName = "Migrations";
	private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);

		try
		{
			using var scope = serviceProvider.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<PgContext>();

			await EnsureDatabaseAsync(dbContext, cancellationToken);
			await RunMigrationAsync(dbContext, cancellationToken);

		}
		catch (Exception ex)
		{
			activity?.RecordException(ex);
			throw;
		}

		hostApplicationLifetime.StopApplication();
	}

	private static async Task EnsureDatabaseAsync(PgContext dbContext, CancellationToken cancellationToken)
	{
		var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

		if (!await dbCreator.ExistsAsync(cancellationToken))
		{
			await dbCreator.CreateAsync(cancellationToken);
		}

	}

	private static async Task RunMigrationAsync(PgContext dbContext, CancellationToken cancellationToken)
	{

		// Run migration in a transaction to avoid partial migration if it fails.
		await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
		await dbContext.Database.MigrateAsync(cancellationToken);
		await transaction.CommitAsync(cancellationToken);

	}



}
