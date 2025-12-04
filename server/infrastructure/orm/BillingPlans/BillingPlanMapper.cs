using LocadoraDeAutomoveis.Domain.BillingPlans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocadoraDeAutomoveis.Infrastructure.BillingPlans;

public class BillingPlanMapper : IEntityTypeConfiguration<BillingPlan>
{
    public void Configure(EntityTypeBuilder<BillingPlan> builder)
    {
        builder.ToTable("BillingPlans");

        builder.HasKey(pp => pp.Id);

        builder.OwnsOne(p => p.DailyPlan, daily =>
        {
            daily.Property(d => d.DailyRate)
                .HasColumnName("DailyPlan_Price")
                .HasPrecision(18, 2)
                .IsRequired();

            daily.Property(d => d.PricePerKm)
                .HasColumnName("DailyPlan_PricePerKm")
                .HasPrecision(18, 2)
                .IsRequired();
        });

        builder.OwnsOne(p => p.ControlledPlan, controlled =>
        {
            controlled.Property(c => c.DailyRate)
                .HasColumnName("ControlledPlan_Price")
                .HasPrecision(18, 2)
                .IsRequired();

            controlled.Property(c => c.PricePerKmExtrapolated)
                .HasColumnName("ControlledPlan_ExtrapolatedPrice")
                .HasPrecision(18, 2)
                .IsRequired();
        });

        builder.OwnsOne(p => p.FreePlan, free =>
        {
            free.Property(f => f.FixedRate)
                .HasColumnName("FreePlan_FixedRate")
                .HasPrecision(18, 2)
                .IsRequired();
        });

        builder.HasOne(pp => pp.Group)
            .WithMany(g => g.BillingPlans)
            .HasForeignKey(pp => pp.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

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
