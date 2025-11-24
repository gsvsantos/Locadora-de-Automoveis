using LocadoraDeAutomoveis.Domain.RateServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocadoraDeAutomoveis.Infrastructure.RateServices;

public class RateServiceMapper : IEntityTypeConfiguration<RateService>
{
    public void Configure(EntityTypeBuilder<RateService> builder)
    {
        builder.ToTable("RateServices");

        builder.HasKey(rS => rS.Id);

        builder.Property(rS => rS.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(rS => rS.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(rS => rS.IsFixed)
            .IsRequired();

        builder.HasOne(rS => rS.Tenant)
            .WithMany()
            .HasForeignKey(rS => rS.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(rS => rS.User)
            .WithMany()
            .HasForeignKey(rS => rS.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(rS => new { rS.TenantId, rS.UserId, rS.IsActive });
    }
}