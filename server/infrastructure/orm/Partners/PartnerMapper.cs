using LocadoraDeAutomoveis.Domain.Partners;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocadoraDeAutomoveis.Infrastructure.Partners;

public class PartnerMapper : IEntityTypeConfiguration<Partner>
{
    public void Configure(EntityTypeBuilder<Partner> builder)
    {
        builder.ToTable("Partners");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.FullName)
            .IsRequired();

        builder.HasMany(p => p.Coupons)
            .WithOne(c => c.Partner)
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
    }
}