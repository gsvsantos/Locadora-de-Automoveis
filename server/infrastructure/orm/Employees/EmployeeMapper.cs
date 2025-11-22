using LocadoraDeAutomoveis.Domain.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocadoraDeAutomoveis.Infrastructure.Employees;

public class EmployeeMapper : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FullName)
            .IsRequired();

        builder.Property(e => e.AdmissionDate)
            .IsRequired();

        builder.Property(e => e.Salary)
               .HasPrecision(18, 2);

        builder.HasOne(t => t.Tenant)
               .WithMany()
               .HasForeignKey(t => t.TenantId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.User)
               .WithOne()
               .HasForeignKey<Employee>(e => e.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(f => new { f.TenantId, f.UserId, f.IsActive });
    }
}