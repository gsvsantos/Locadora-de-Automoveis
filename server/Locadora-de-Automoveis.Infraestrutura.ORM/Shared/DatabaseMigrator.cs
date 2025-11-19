using Microsoft.EntityFrameworkCore;

namespace Locadora_de_Automoveis.Infraestrutura.ORM.Shared;

public static class DatabaseMigrator
{
    public static bool AutoDatabaseUpdate(DbContext dbContext)
    {
        int migrationQueueInt = dbContext.Database.GetPendingMigrations().Count();

        if (migrationQueueInt == 0)
        {
            return false;
        }

        dbContext.Database.Migrate();

        return true;
    }
}
