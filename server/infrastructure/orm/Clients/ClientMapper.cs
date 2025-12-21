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
            a.Property(a => a.State)
            .IsRequired();

            a.Property(a => a.City)
            .IsRequired();

            a.Property(a => a.Neighborhood)
            .IsRequired();

            a.Property(a => a.Street)
            .IsRequired();

            a.Property(a => a.Number)
            .IsRequired();
        });

        builder.Navigation(c => c.Address)
            .IsRequired(false);

        builder.Property(c => c.Document);

        builder.Property(c => c.LicenseNumber);

        builder.Property(c => c.LicenseValidity);

        builder.HasOne(c => c.JuristicClient)
            .WithMany()
            .HasForeignKey(c => c.JuristicClientId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.Property(c => c.PreferredLanguage)
            .IsRequired();

        builder.Property(c => c.TenantId)
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

        builder.HasIndex(client => new { client.TenantId, client.LoginUserId })
             .IsUnique()
             .HasFilter("[TenantId] IS NOT NULL AND [LoginUserId] IS NOT NULL");

        builder.HasIndex(client => client.LoginUserId)
            .IsUnique()
            .HasFilter("[TenantId] IS NULL AND [LoginUserId] IS NOT NULL");

        builder.HasIndex(client => new { client.Document, client.TenantId })
            .IsUnique()
            .HasFilter("[TenantId] IS NOT NULL AND [Document] IS NOT NULL");
    }
}