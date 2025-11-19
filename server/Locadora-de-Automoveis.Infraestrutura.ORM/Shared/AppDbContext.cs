using Locadora_de_Automoveis.Core.Dominio.Auth;
using Locadora_de_Automoveis.Core.Dominio.Shared;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;

namespace Locadora_de_Automoveis.Infraestrutura.ORM.Shared;

public class AppDbContext(DbContextOptions options, ITenantProvider? tenantProvider = null)
    : IdentityDbContext<User, Role, Guid>(options), IUnitOfWork
{

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (tenantProvider is not null)
        {
        }

        Assembly assembly = typeof(AppDbContext).Assembly;

        modelBuilder.ApplyConfigurationsFromAssembly(assembly);

        base.OnModelCreating(modelBuilder);
    }

    public async Task CommitAsync() => await SaveChangesAsync();

    public async Task RollbackAsync()
    {
        foreach (EntityEntry entry in this.ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.State = EntityState.Unchanged;
                    break;

                case EntityState.Modified:
                    entry.State = EntityState.Unchanged;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Unchanged;
                    break;
            }
        }

        await Task.CompletedTask;
    }
}
