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

        builder.OwnsOne(p => p.DailyPlan, daily =>
        {
            daily.Property(d => d.DailyRate)
                .HasColumnName("DailyPlan_Price");
            daily.Property(d => d.PricePerKm)
                .HasColumnName("DailyPlan_PricePerKm");
        });

        builder.OwnsOne(p => p.ControlledPlan, controlled =>
        {
            controlled.Property(c => c.DailyRate)
                .HasColumnName("ControlledPlan_Price");
            controlled.Property(c => c.AvailableKm)
                .HasColumnName("ControlledPlan_AvailableKm");
            controlled.Property(c => c.PricePerKmExtrapolated)
                .HasColumnName("ControlledPlan_ExtrapolatedPrice");
        });

        builder.OwnsOne(p => p.FreePlan, free =>
        {
            free.Property(f => f.FixedRate).HasColumnName("FreePlan_FixedRate");
        });

        builder.HasOne(pp => pp.Group)
            .WithMany()
            .HasForeignKey(pp => pp.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
