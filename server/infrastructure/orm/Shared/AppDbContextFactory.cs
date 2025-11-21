using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Shared;

public static class AppDbContextFactory
{
    public static AppDbContext CreateDbContext(string connectionString)
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString, opt =>
            {
                opt.EnableRetryOnFailure(3);
                opt.CommandTimeout(60);
            })
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .Options;

        return new(options);
    }
}
