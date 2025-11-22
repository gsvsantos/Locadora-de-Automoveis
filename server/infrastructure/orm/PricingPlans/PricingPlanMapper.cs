using LocadoraDeAutomoveis.Domain.PricingPlans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocadoraDeAutomoveis.Infrastructure.PricingPlans;

public class PricingPlanMapper : IEntityTypeConfiguration<PricingPlan>
{
    public void Configure(EntityTypeBuilder<PricingPlan> builder)
    {
        builder.ToTable("PricingPlans");

        builder.HasKey(pp => pp.Id);

        builder.Property(pp => pp.DailyRate)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(pp => pp.PricePerKm)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(pp => pp.AvailableKm)
            .IsRequired();

        builder.Property(pp => pp.DailyPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(pp => pp.PricePerKmExtrapolated)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(pp => pp.FixedRate)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne(pp => pp.Group)
            .WithMany()
            .HasForeignKey(pp => pp.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
