using LocadoraDeAutomoveis.Domain.Rentals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocadoraDeAutomoveis.Infrastructure.Rentals;

public class RentalReturnMapper : IEntityTypeConfiguration<RentalReturn>
{
    public void Configure(EntityTypeBuilder<RentalReturn> builder)
    {
        builder.ToTable("RentalReturns");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.ReturnDate)
            .IsRequired();

        builder.Property(r => r.EndKm)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(r => r.TotalMileage)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(r => r.ExtrasTotalCost)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(r => r.FuelPenalty)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(r => r.PenaltyTotalCost)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(r => r.DiscountTotal)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(r => r.FinalPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(r => r.FuelLevelAtReturn)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(r => r.Rental)
            .WithOne()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Tenant)
            .WithMany()
            .HasForeignKey(r => r.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.TenantId, r.UserId, r.IsActive });

        builder.HasIndex(r => r.RentalId).IsUnique();
    }
}
