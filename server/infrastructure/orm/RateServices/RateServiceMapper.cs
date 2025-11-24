using LocadoraDeAutomoveis.Domain.RateService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocadoraDeAutomoveis.Infrastructure.RateServices;

public class RateServiceMapper : IEntityTypeConfiguration<RateService>
{
    public void Configure(EntityTypeBuilder<RateService> builder)
    {
        builder.ToTable("RateServices");

        builder.HasKey(rs => rs.Id);

        builder.Property(rs => rs.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(rs => rs.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(rs => rs.IsFixed)
            .IsRequired();

        builder.HasOne(rs => rs.Tenant)
            .WithMany()
            .HasForeignKey(t => t.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(rs => rs.User)
            .WithMany()
            .HasForeignKey(rs => rs.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(rs => new { rs.TenantId, rs.UserId, rs.IsActive });
    }
}