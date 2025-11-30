using LocadoraDeAutomoveis.Domain.Rentals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocadoraDeAutomoveis.Infrastructure.Rentals;

public class RentalMapper : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> builder)
    {
        builder.ToTable("Rentals");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.StartDate)
            .IsRequired();

        builder.Property(r => r.ExpectedReturnDate)
            .IsRequired();

        builder.Property(r => r.ReturnDate)
            .IsRequired(false);

        builder.Property(r => r.StartKm)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(r => r.BaseRentalPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(r => r.GuaranteeValue)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(r => r.FinalPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(r => r.EstimatedKilometers)
            .IsRequired(false);

        builder.Property(r => r.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(r => r.SelectedPlanType)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(r => r.Client)
            .WithMany()
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Driver)
            .WithMany()
            .HasForeignKey(r => r.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Vehicle)
            .WithMany()
            .HasForeignKey(r => r.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.PricingPlan)
            .WithMany()
            .HasForeignKey(r => r.PricingPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Employee)
            .WithMany()
            .HasForeignKey(r => r.EmployeeId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.RateServices)
            .WithOne()
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne(t => t.Tenant)
            .WithMany()
            .HasForeignKey(t => t.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.User)
            .WithMany()
            .HasForeignKey(g => g.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(f => new { f.TenantId, f.UserId, f.IsActive });
    }
}