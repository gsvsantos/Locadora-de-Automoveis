using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using LocadoraDeAutomoveis.Tests.Integration.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Tests.Integration.Repositories;

[TestClass]
[TestCategory("BaseRepository Infrastructure - Integration Tests")]
public sealed class BaseRepositoryTests : TestFixture
{
    private BaseRepository<TestEntity> baseRepositoryTest = null!;

    [TestInitialize]
    public override void ConfigurarTestes()
    {
        base.ConfigurarTestes();

        this.baseRepositoryTest = new BaseRepository<TestEntity>(this.dbContext);

        this.dbContext.Set<TestEntity>().RemoveRange(this.dbContext.Set<TestEntity>());

        this.dbContext.SaveChanges();
    }

    [TestMethod]
    public async Task Should_AddAsync_And_GetFromDb_Successfully()
    {
        // Arrange
        User tenant = Builder<User>.CreateNew()
            .With(t => t.FullName == "Tenant User")
            .With(t => t.UserName = $"tenantUser-{Guid.NewGuid()}")
            .With(t => t.Id == Guid.NewGuid()).Persist();
        tenant.AssociateTenant(tenant.Id);

        TestEntity entity = Builder<TestEntity>.CreateNew()
            .With(x => x.Name = $"Entity-{Guid.NewGuid()}")
            .With(x => x.Age = 30)
            .Build();
        entity.AssociateTenant(tenant.Id);
        entity.AssociateUser(tenant);

        // Act
        await this.baseRepositoryTest.AddAsync(entity);
        await this.dbContext.SaveChangesAsync();

        // Assert
        TestEntity? saved = await this.dbContext.Set<TestEntity>().FirstOrDefaultAsync(x => x.Id == entity.Id);

        Assert.IsNotNull(saved);
        Assert.AreEqual(entity.Name, saved!.Name);
    }

    [TestMethod]
    public async Task Should_AddMultiplyAsync_Successfully()
    {
        // Arrange
        User tenant = Builder<User>.CreateNew()
            .With(t => t.FullName == "Tenant User")
            .With(t => t.UserName = $"tenantUser-{Guid.NewGuid()}")
            .With(t => t.Id == Guid.NewGuid()).Persist();
        tenant.AssociateTenant(tenant.Id);

        IList<TestEntity> entities = Builder<TestEntity>.CreateListOfSize(5)
            .All().With(x => x.Name = $"Entity-{Guid.NewGuid()}")
            .Build();

        foreach (TestEntity entity in entities)
        {
            entity.AssociateTenant(tenant.Id);
            entity.AssociateUser(tenant);
        }

        // Act
        await this.baseRepositoryTest.AddMultiplyAsync(entities);
        await this.dbContext.SaveChangesAsync();

        // Assert
        int count = await this.dbContext.Set<TestEntity>().CountAsync();
        Assert.AreEqual(5, count);
    }

    [TestMethod]
    public async Task Should_UpdateAsync_Successfully()
    {
        // Arrange
        User tenant = Builder<User>.CreateNew()
            .With(t => t.FullName == "Tenant User")
            .With(t => t.UserName = $"tenantUser-{Guid.NewGuid()}")
            .With(t => t.Id == Guid.NewGuid()).Persist();
        tenant.AssociateTenant(tenant.Id);

        TestEntity entity = Builder<TestEntity>.CreateNew()
            .With(x => x.Name = $"Entity-{Guid.NewGuid()}")
            .With(x => x.Age = 30)
            .Build();
        entity.AssociateTenant(tenant.Id);
        entity.AssociateUser(tenant);

        await this.baseRepositoryTest.AddAsync(entity);
        await this.dbContext.SaveChangesAsync();

        TestEntity updated = new() { Name = $"Entity-{Guid.NewGuid()}", Age = 35 };

        // Act
        bool result = await this.baseRepositoryTest.UpdateAsync(entity.Id, updated);

        TestEntity? fromDb = await this.baseRepositoryTest.GetByIdAsync(entity.Id);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(updated.Name, fromDb!.Name);
        Assert.AreEqual(35, fromDb!.Age);
    }

    [TestMethod]
    public async Task Should_UpdateAsync_Unsuccessfully()
    {
        bool result = await this.baseRepositoryTest.UpdateAsync(Guid.NewGuid(), new TestEntity());

        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Should_DeleteAsync_Successfully()
    {
        // Arrange
        User tenant = Builder<User>.CreateNew()
            .With(t => t.FullName == "Tenant User")
            .With(t => t.UserName = $"tenantUser-{Guid.NewGuid()}")
            .With(t => t.Id == Guid.NewGuid()).Persist();
        tenant.AssociateTenant(tenant.Id);

        TestEntity entity = Builder<TestEntity>.CreateNew()
            .With(x => x.Name = $"Entity-{Guid.NewGuid()}")
            .With(x => x.Age = 30)
            .Build();
        entity.AssociateTenant(tenant.Id);
        entity.AssociateUser(tenant);

        await this.baseRepositoryTest.AddAsync(entity);
        await this.dbContext.SaveChangesAsync();

        // Act
        bool result = await this.baseRepositoryTest.DeleteAsync(entity.Id);
        await this.dbContext.SaveChangesAsync();

        bool exists = await this.dbContext.Set<TestEntity>().AnyAsync(x => x.Id == entity.Id);

        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(exists);
    }

    [TestMethod]
    public async Task Should_DeleteAsync_Unsuccessfully()
    {
        // Arrange & Act
        bool result = await this.baseRepositoryTest.DeleteAsync(Guid.NewGuid());

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Should_GetAllAsync_Successfully()
    {
        // Arrange
        User tenant = Builder<User>.CreateNew()
            .With(t => t.FullName == "Tenant User")
            .With(t => t.UserName = $"tenantUser-{Guid.NewGuid()}")
            .With(t => t.Id == Guid.NewGuid()).Persist();
        tenant.AssociateTenant(tenant.Id);

        IList<TestEntity> entities = Builder<TestEntity>.CreateListOfSize(10)
            .All().With(x => x.Name = $"Entity-{Guid.NewGuid()}")
            .Build();

        foreach (TestEntity entity in entities)
        {
            entity.AssociateTenant(tenant.Id);
            entity.AssociateUser(tenant);
        }

        await this.baseRepositoryTest.AddMultiplyAsync(entities);
        await this.dbContext.SaveChangesAsync();

        // Act
        List<TestEntity> result = await this.baseRepositoryTest.GetAllAsync();

        // Assert
        Assert.AreEqual(10, result.Count);
    }

    [TestMethod]
    public async Task Should_GetAllAsync_WithQuantity_Successfully()
    {
        // Arrange
        User tenant = Builder<User>.CreateNew()
            .With(t => t.FullName == "Tenant User")
            .With(t => t.UserName = $"tenantUser-{Guid.NewGuid()}")
            .With(t => t.Id == Guid.NewGuid()).Persist();
        tenant.AssociateTenant(tenant.Id);

        IList<TestEntity> entities = Builder<TestEntity>.CreateListOfSize(10)
            .All().With(x => x.Name = $"Entity-{Guid.NewGuid()}")
            .Build();

        foreach (TestEntity entity in entities)
        {
            entity.AssociateTenant(tenant.Id);
            entity.AssociateUser(tenant);
        }

        await this.baseRepositoryTest.AddMultiplyAsync(entities);
        await this.dbContext.SaveChangesAsync();

        // Act
        List<TestEntity> result = await this.baseRepositoryTest.GetAllAsync(5);

        // Assert
        Assert.AreEqual(5, result.Count);
    }

    [TestMethod]
    public async Task Should_GetByIdAsync_Successfully()
    {
        // Arrange
        User tenant = Builder<User>.CreateNew()
            .With(t => t.FullName == "Tenant User")
            .With(t => t.UserName = $"tenantUser-{Guid.NewGuid()}")
            .With(t => t.Id == Guid.NewGuid()).Persist();
        tenant.AssociateTenant(tenant.Id);

        TestEntity entity = Builder<TestEntity>.CreateNew()
            .With(x => x.Name = $"Entity-{Guid.NewGuid()}")
            .With(x => x.Age = 30)
            .Build();
        entity.AssociateTenant(tenant.Id);
        entity.AssociateUser(tenant);

        await this.baseRepositoryTest.AddAsync(entity);
        await this.dbContext.SaveChangesAsync();

        // Act
        TestEntity? found = await this.baseRepositoryTest.GetByIdAsync(entity.Id);

        // Assert
        Assert.IsNotNull(found);
        Assert.AreEqual(entity.Name, found!.Name);
    }
}
