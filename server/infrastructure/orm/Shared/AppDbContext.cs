using LocadoraDeAutomoveis.Core.Domain.Auth;
using LocadoraDeAutomoveis.Core.Domain.Shared;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;

namespace LocadoraDeAutomoveis.Infrastructure.ORM.Shared;

public class AppDbContext(DbContextOptions options, ITenantProvider? tenantProvider = null)
    : IdentityDbContext<User, Role, Guid>(options), IUnitOfWork
{

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (tenantProvider is not null)
        {
            Guid userId = tenantProvider.GetTenantId();

            //modelBuilder.Entity<Exemplo>()
            //    .HasQueryFilter(x => x.TenantId.Equals(userId));
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
