using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LocadoraDeAutomoveis.Infrastructure.Shared;

public sealed class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        string? sqlConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(sqlConnectionString))
        {
            throw new InvalidOperationException("The environment variable \"SQL_CONNECTION_STRING\" was not provided.");
        }

        DbContextOptions<AppDbContext> dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(sqlConnectionString, sqlServerOptions =>
            {
                sqlServerOptions.EnableRetryOnFailure(3);
            })
            .Options;

        return new AppDbContext(dbContextOptions);
    }
}
