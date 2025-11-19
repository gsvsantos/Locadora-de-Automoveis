using Microsoft.EntityFrameworkCore;

namespace Locadora_de_Automoveis.Infraestrutura.ORM.Shared;

public static class AppDbContextFactory
{
    public static AppDbContext CreateDbContext(string connectionString)
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString, opt =>
            {
                opt.EnableRetryOnFailure(3);
            })
            .Options;

        return new(options);
    }
}
