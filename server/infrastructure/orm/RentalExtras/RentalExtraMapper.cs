using LocadoraDeAutomoveis.Domain.RentalExtras;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocadoraDeAutomoveis.Infrastructure.RentalExtras;

public class RentalExtraMapper : IEntityTypeConfiguration<RentalExtra>
{
    public void Configure(EntityTypeBuilder<RentalExtra> builder)
    {
        builder.ToTable("Extras");

        builder.HasKey(rS => rS.Id);

        builder.Property(rS => rS.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(rS => rS.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(rS => rS.IsDaily)
            .IsRequired();

        builder.Property(rS => rS.Type)
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