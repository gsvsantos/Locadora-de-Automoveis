using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Base;

public class TestEntity : BaseEntity<TestEntity>
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }

    public void AssociateTenant(Guid tenantId) => this.TenantId = tenantId;

    public override void Update(TestEntity source)
    {
        this.Name = source.Name;
        this.Age = source.Age;
    }
}