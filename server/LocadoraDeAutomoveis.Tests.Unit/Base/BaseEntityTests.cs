namespace LocadoraDeAutomoveis.Tests.Unit.Base;

[TestClass]
[TestCategory("BaseEntity Domain - Unit Tests")]
public sealed class BaseEntityTests
{
    [TestMethod]
    public void EmployeeConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        TestEntity entity = new();

        // Assert
        Assert.AreNotEqual(Guid.Empty, entity.Id);
        Assert.AreEqual(Guid.Empty, entity.TenantId);
        Assert.IsNull(entity.Tenant);
        Assert.AreEqual(string.Empty, entity.Name);
        Assert.AreEqual(0, entity.Age);
    }

    [TestMethod]
    public void EmployeeConstructor_Parameterized_ShouldWorks()
    {
        // Arrange & Act
        DateTimeOffset admissionDate = DateTimeOffset.UtcNow;

        TestEntity entity = new()
        {
            Name = "Entity Teste da Silva",
            Age = 33
        };

        // Assert
        Assert.AreNotEqual(Guid.Empty, entity.Id);
        Assert.AreEqual(Guid.Empty, entity.TenantId);
        Assert.IsNull(entity.Tenant);
        Assert.AreEqual("Entity Teste da Silva", entity.Name);
        Assert.AreEqual(33, entity.Age);
    }
}
