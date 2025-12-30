using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Domain.Auth;

namespace LocadoraDeAutomoveis.Tests.Unit.Base.Domain;

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
        Assert.AreEqual(null, entity.TenantId);
        Assert.IsNull(entity.Tenant);
        Assert.AreEqual(string.Empty, entity.Name);
        Assert.AreEqual(0, entity.Age);
    }

    [TestMethod]
    public void EntityConstructor_Parameterized_ShouldWorks()
    {
        // Arrange & Act
        TestEntity entity = new()
        {
            Name = "Entity Teste da Silva",
            Age = 33
        };

        // Assert
        Assert.AreNotEqual(Guid.Empty, entity.Id);
        Assert.AreEqual(null, entity.TenantId);
        Assert.IsNull(entity.Tenant);
        Assert.AreEqual("Entity Teste da Silva", entity.Name);
        Assert.AreEqual(33, entity.Age);
    }

    [TestMethod]
    public void EntityMethod_Update_ShouldWorks()
    {
        // Arrange
        TestEntity entity = new()
        {
            Name = "Entity Teste da Silva",
            Age = 33
        };

        TestEntity updatedEntity = new()
        {
            Name = "Entity Teste da Silva EDITADO",
            Age = 44
        };

        // Act
        entity.Update(updatedEntity);

        // Assert
        Assert.AreEqual(updatedEntity.Name, entity.Name);
        Assert.AreEqual(updatedEntity.Age, entity.Age);
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

    [TestMethod]
    public void EntityMethod_AssociateUser_ShouldWorks()
    {
        // Arrange & Act
        User user = new();

        TestEntity entity = new();
        entity.AssociateUser(user);

        // Assert
        Assert.AreEqual(user, entity.User);
        Assert.IsNotNull(entity.User);
        Assert.AreEqual(user.Id, entity.User.Id);
    }

    [TestMethod]
    public void EntityMethod_AssociateUser_ShouldNotWork()
    {
        // Arrange & Act
        TestEntity entity = new();
        entity.AssociateUser(null);

        // Assert
        Assert.IsNull(entity.User);
        Assert.AreEqual(Guid.Empty, entity.UserId);
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
    public void EntityMethod_AssociateTenant_SameTenant_ShouldReturn()
    {
        // Arrange
        Guid tenantId = Guid.NewGuid();
        TestEntity entity = new();
        entity.AssociateTenant(tenantId);

        // Act
        entity.AssociateTenant(tenantId);

        // Assert
        Assert.AreEqual(tenantId, entity.TenantId);
    }

    [TestMethod]
    public void EntityMethod_AssociateTenant_EmptyTenantId_ShouldWorks()
    {
        // Arrange
        Guid tenantId = Guid.NewGuid();
        TestEntity entity = new();
        entity.AssociateTenant(tenantId);

        // Act
        entity.AssociateTenant(Guid.Empty);

        // Assert
        Assert.IsNull(entity.TenantId);
        Assert.IsNull(entity.Tenant);
    }

    [TestMethod]
    public void EntityMethod_AssociateTenant_DiffTenant_ShouldWorks()
    {
        // Arrange
        Guid tenantId = Guid.NewGuid();
        User tenant = Builder<User>.CreateNew()
            .With(u => u.Id = tenantId)
            .Build();

        TestEntity entity = new() { Tenant = tenant, TenantId = tenantId };

        // Act
        entity.AssociateTenant(Guid.Empty);

        // Assert
        Assert.IsNull(entity.TenantId);
        Assert.IsNull(entity.Tenant);
    }

    [TestMethod]
    public void EntityMethod_AssociateTenant_ShouldKeepTenantProp()
    {
        // Arrange
        Guid tenantId = Guid.NewGuid();
        User tenant = Builder<User>.CreateNew()
            .With(u => u.Id = tenantId)
            .Build();

        TestEntity entity = new() { Tenant = tenant, TenantId = null };

        // Act
        entity.AssociateTenant(tenantId);

        // Assert
        Assert.AreEqual(tenantId, entity.TenantId);
        Assert.IsNotNull(entity.Tenant);
        Assert.AreEqual(tenant, entity.Tenant);
    }

    [TestMethod]
    public void EntityMethod_AssociateTenant_ShouldClearTenant()
    {
        // Arrange
        Guid tenantId = Guid.NewGuid();
        User tenant = Builder<User>.CreateNew()
            .With(u => u.Id = tenantId)
            .Build();

        TestEntity entity = new() { Tenant = tenant, TenantId = null };

        // Act
        entity.AssociateTenant(Guid.Empty);

        // Assert
        Assert.IsNull(entity.TenantId);
        Assert.IsNotNull(entity.Tenant);
        Assert.AreEqual(tenant, entity.Tenant);
    }

    [TestMethod]
    public void EntityMethod_GetTenantId_ShouldWorks()
    {
        // Arrange
        Guid tenantId = Guid.NewGuid();
        TestEntity entityWithTenant = new();
        entityWithTenant.AssociateTenant(tenantId);

        TestEntity entityWithoutTenant = new();

        // Act
        Guid retrievedTenantId = entityWithTenant.GetTenantId();
        Guid retrievedEmptyTenantId = entityWithoutTenant.GetTenantId();

        // Assert
        Assert.AreEqual(tenantId, retrievedTenantId);
        Assert.AreEqual(Guid.Empty, retrievedEmptyTenantId);
    }
}
