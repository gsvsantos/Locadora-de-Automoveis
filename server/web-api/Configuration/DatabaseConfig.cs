using Locadora_de_Automoveis.Infraestrutura.ORM.Shared;

namespace Locadora_de_Automoveis.WebAPI.Configuration;

public static class DatabaseConfig
{
    public static bool AutoMigrateDatabase(this IHost app)
    {
        using IServiceScope scope = app.Services.CreateScope();

        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        bool success = DatabaseMigrator.AutoDatabaseUpdate(dbContext);

        return success;
    }
}