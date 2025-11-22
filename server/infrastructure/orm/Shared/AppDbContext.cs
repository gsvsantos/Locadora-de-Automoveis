using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;

namespace LocadoraDeAutomoveis.Infrastructure.Shared;

public class AppDbContext(DbContextOptions options, ITenantProvider? tenantProvider = null)
    : IdentityDbContext<User, Role, Guid>(options), IUnitOfWork
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (tenantProvider is not null)
        {
            modelBuilder.Entity<Employee>()
                .HasQueryFilter(x => x.TenantId.Equals(tenantProvider.GetTenantId()));
            modelBuilder.Entity<Group>()
                .HasQueryFilter(x => x.TenantId.Equals(tenantProvider.GetTenantId()));
            modelBuilder.Entity<Vehicle>()
                .HasQueryFilter(x => x.TenantId.Equals(tenantProvider.GetTenantId()));
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
