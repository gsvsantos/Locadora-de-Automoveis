using LocadoraDeAutomoveis.Domain.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocadoraDeAutomoveis.Infrastructure.Clients;

public class ClientMapper : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FullName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasMaxLength(254)
            .IsRequired();

        builder.Property(c => c.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.IsJuridical)
            .IsRequired();

        builder.Property(c => c.State)
            .IsRequired();

        builder.Property(c => c.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Neighborhood)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Street)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Number)
            .IsRequired();

        builder.Property(c => c.Document);

        builder.Property(c => c.LicenseNumber);

        builder.HasOne(c => c.Tenant)
            .WithMany()
            .HasForeignKey(c => c.TenantId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.TenantId, c.UserId, c.IsActive });

        builder.HasIndex(c => new { c.Document, c.TenantId })
            .IsUnique()
            .HasFilter("[Document] IS NOT NULL");
    }
}