using Hangfire;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using LocadoraDeAutomoveis.WebApi.Jobs;

namespace LocadoraDeAutomoveis.WebAPI.Configuration;

public static class DatabaseConfig
{
    public static bool AutoMigrateDatabase(this IHost app)
    {
        using IServiceScope scope = app.Services.CreateScope();

        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        bool success = DatabaseMigrator.AutoDatabaseUpdate(dbContext);

        return success;
    }

    public static void StartRefreshTokenCleanupJob(this WebApplication app)
    {
        IRecurringJobManager recurringJobManager = app.Services
            .GetRequiredService<IRecurringJobManager>();

        recurringJobManager.AddOrUpdate<RefreshTokenCleanupJob>(
            "refresh-token-cleanup",
            job => job.ExecuteAsync(CancellationToken.None),
            Cron.Daily
        );
    }
}