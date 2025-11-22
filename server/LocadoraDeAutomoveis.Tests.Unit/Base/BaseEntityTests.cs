using LocadoraDeAutomoveis.Domain.Auth;

namespace LocadoraDeAutomoveis.Tests.Unit.Base;

[TestClass]
[TestCategory("BaseEntity Domain - Unit Tests")]
public sealed class BaseEntityTests
{
    [TestMethod]
    public void EntityConstructor_Default_ShouldInitializeProperties()
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
    public void EntityConstructor_Parameterized_ShouldWorks()
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

    [TestMethod]
    public void EntityMethod_AssociateUser_ShouldWorks()
    {
        // Arrange & Act
        User user = new();

        TestEntity entity = new();
        entity.AssociateUser(user);

        // Assert
        Assert.AreEqual(user, entity.User);
        Assert.AreEqual(user.Id, entity.User!.Id);
    }

    [TestMethod]
    public void EntityMethod_AssociateTenant_ShouldWorks()
    {
        // Arrange & Act
        Guid tenantId = Guid.NewGuid();

        TestEntity entity = new();
        entity.AssociateTenant(tenantId);

        // Assert
        Assert.AreEqual(tenantId, entity.TenantId);
    }

    [TestMethod]
    public void EntityMethod_Deactivate_ShouldWorks()
    {
        // Arrange & Act
        TestEntity entity = new();
        entity.Deactivate();

        // Assert
        Assert.IsFalse(entity.IsActive);
    }
}
