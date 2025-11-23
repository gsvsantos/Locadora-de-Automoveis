using LocadoraDeAutomoveis.Domain.Drivers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocadoraDeAutomoveis.Infrastructure.Drivers;

public class DriverMapper : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.ToTable("Drivers");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.FullName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(d => d.Email)
            .HasMaxLength(254)
            .IsRequired();

        builder.Property(d => d.PhoneNumber)
            .IsRequired();

        builder.Property(d => d.Document)
            .IsRequired();

        builder.Property(d => d.LicenseNumber)
            .IsRequired();

        builder.Property(d => d.LicenseValidity)
            .IsRequired();

        builder.HasOne(d => d.ClientCPF)
            .WithMany()
            .HasForeignKey(d => d.ClientCPFId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.ClientCNPJ)
            .WithMany()
            .HasForeignKey(d => d.ClientCNPJId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne(d => d.Tenant)
            .WithMany()
            .HasForeignKey(d => d.TenantId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => new { d.TenantId, d.UserId, d.IsActive });

        builder.HasIndex(d => new { d.Document, d.TenantId }).IsUnique();
        builder.HasIndex(d => new { d.LicenseNumber, d.TenantId }).IsUnique();
    }
}
