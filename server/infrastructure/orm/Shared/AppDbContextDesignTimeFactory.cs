using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LocadoraDeAutomoveis.Infrastructure.Shared;

public sealed class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .AddUserSecrets<AppDbContextDesignTimeFactory>()
            .Build();

        string? sqlConnectionString = configuration["SQL_CONNECTION_STRING"];

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
