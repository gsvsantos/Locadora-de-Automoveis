using LocadoraDeAutomoveis.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocadoraDeAutomoveis.Infrastructure.Configurations;

public class ConfigurationMapper : IEntityTypeConfiguration<Configuration>
{
    public void Configure(EntityTypeBuilder<Configuration> builder)
    {
        builder.ToTable("Configurations");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.GasolinePrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(c => c.GasPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(c => c.DieselPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(c => c.AlcoholPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(c => c.Tenant)
            .WithMany()
            .HasForeignKey(t => t.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.TenantId, c.UserId, c.IsActive });
    }
}