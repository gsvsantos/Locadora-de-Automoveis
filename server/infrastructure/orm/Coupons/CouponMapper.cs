using LocadoraDeAutomoveis.Domain.Coupons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocadoraDeAutomoveis.Infrastructure.Coupons;

public class CouponMapper : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("Coupons");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.DiscountValue)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.ExpirationDate)
            .IsRequired();

        builder.Property(c => c.IsManuallyDisabled)
            .IsRequired();

        builder.HasOne(c => c.Partner)
            .WithMany()
            .HasForeignKey(c => c.PartnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.TenantId)
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

        builder.HasIndex(c => new { c.TenantId, c.Name })
            .IsUnique();
    }
}