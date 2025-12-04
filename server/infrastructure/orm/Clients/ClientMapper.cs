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

        builder.Property(c => c.Type)
            .HasColumnType("int")
            .IsRequired();

        builder.OwnsOne(c => c.Address, a =>
        {
            a.Property(p => p.State)
            .IsRequired();

            a.Property(p => p.City)
            .IsRequired();

            a.Property(p => p.Neighborhood)
            .IsRequired();

            a.Property(p => p.Street)
            .IsRequired();

            a.Property(p => p.Number)
            .IsRequired();
        });

        builder.Property(c => c.Document);

        builder.Property(c => c.LicenseNumber);

        builder.HasOne(d => d.JuristicClient)
            .WithMany()
            .HasForeignKey(d => d.JuristicClientId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

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
            .IsUnique();
    }
}