using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using LocadoraDeAutomoveis.Tests.Integration.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Tests.Integration.Shared;

public sealed class TestAppDbContext : AppDbContext
{
    public TestAppDbContext(DbContextOptions options) : base(options) { }

    public DbSet<TestEntity> TestEntities
    {
        get
        {
            return Set<TestEntity>();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TestEntity>(builder =>
        {
            builder.ToTable("TestEntities");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasColumnType("nvarchar(max)");
            builder.Property(x => x.Age);

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(x => x.TenantId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        });
    }
    public static TestAppDbContext CreateDbContext(string connectionString)
    {
        DbContextOptions<TestAppDbContext> options = new DbContextOptionsBuilder<TestAppDbContext>()
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
