using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Base;

public class TestEntity : BaseEntity<TestEntity>
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }

    public override void Update(TestEntity updatedEntity)
    {
        this.Name = updatedEntity.Name;
        this.Age = updatedEntity.Age;
    }
}