using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.RentalExtras;

namespace LocadoraDeAutomoveis.Tests.Integration.Repositories;

[TestClass]
[TestCategory("RentalExtraRepository Infrastructure - Integration Tests")]
public sealed class RentalExtraRepositoryTests : TestFixture
{
    [TestMethod]
    public async Task Should_GetAllAsync_ReturnRentalExtras_Successfuly()
    {
        // Arrange
        User tenant = Builder<User>.CreateNew()
            .With(t => t.FullName = "Tenant User")
            .With(t => t.UserName = $"tenantUser-{Guid.NewGuid()}")
            .With(t => t.Id = Guid.NewGuid()).Persist();
        tenant.AssociateTenant(tenant.Id);

        User userEmployee = Builder<User>.CreateNew()
            .With(uE => uE.FullName = "Employee User")
            .With(uE => uE.UserName = $"employeeUser-{Guid.NewGuid()}")
            .With(uE => uE.Id = Guid.NewGuid()).Persist();
        userEmployee.AssociateTenant(tenant.Id);

        List<RentalExtra> existingRentalExtras = Builder<RentalExtra>.CreateListOfSize(10).Build().ToList();
        foreach (RentalExtra RentalExtra in existingRentalExtras)
        {
            RentalExtra.AssociateTenant(tenant.Id);
            RentalExtra.AssociateUser(userEmployee);
        }

        await this.rentalExtraRepository.AddMultiplyAsync(existingRentalExtras);

        await this.dbContext.SaveChangesAsync();

        // Act
        List<RentalExtra> RentalExtras = await this.rentalExtraRepository.GetAllAsync();

        // Assert
        CollectionAssert.AreEquivalent(existingRentalExtras, RentalExtras);
        Assert.AreEqual(10, RentalExtras.Count);
    }

    [TestMethod]
    public async Task Should_GetFiveAsync_ReturnRentalExtras_Successfuly()
    {
        // Arrange
        User tenant = Builder<User>.CreateNew()
            .With(t => t.FullName = "Tenant User")
            .With(t => t.UserName = $"tenantUser-{Guid.NewGuid()}")
            .With(t => t.Id = Guid.NewGuid()).Persist();
        tenant.AssociateTenant(tenant.Id);

        User userEmployee = Builder<User>.CreateNew()
            .With(uE => uE.FullName = "Employee User")
            .With(uE => uE.UserName = $"employeeUser-{Guid.NewGuid()}")
            .With(uE => uE.Id = Guid.NewGuid()).Persist();
        userEmployee.AssociateTenant(tenant.Id);

        List<RentalExtra> existingRentalExtras = Builder<RentalExtra>.CreateListOfSize(10).Build().ToList();
        foreach (RentalExtra RentalExtra in existingRentalExtras)
        {
            RentalExtra.AssociateTenant(tenant.Id);
            RentalExtra.AssociateUser(userEmployee);
        }

        await this.rentalExtraRepository.AddMultiplyAsync(existingRentalExtras);

        await this.dbContext.SaveChangesAsync();

        // Act
        List<RentalExtra> RentalExtras = await this.rentalExtraRepository.GetAllAsync(5);

        // Assert
        Assert.AreEqual(5, RentalExtras.Count);
    }

    [TestMethod]
    public async Task Should_GetGroupById_ReturnRentalExtra_Successfuly()
    {
        // Arrange
        User tenant = Builder<User>.CreateNew()
            .With(t => t.FullName = "Tenant User")
            .With(t => t.UserName = $"tenantUser-{Guid.NewGuid()}")
            .With(t => t.Id = Guid.NewGuid()).Persist();
        tenant.AssociateTenant(tenant.Id);

        User userEmployee = Builder<User>.CreateNew()
            .With(uE => uE.FullName = "Employee User")
            .With(uE => uE.UserName = $"employeeUser-{Guid.NewGuid()}")
            .With(uE => uE.Id = Guid.NewGuid()).Persist();
        userEmployee.AssociateTenant(tenant.Id);

        RentalExtra RentalExtra = Builder<RentalExtra>.CreateNew().Build();
        RentalExtra.AssociateTenant(tenant.Id);
        RentalExtra.AssociateUser(userEmployee);

        await this.rentalExtraRepository.AddAsync(RentalExtra);

        await this.dbContext.SaveChangesAsync();

        // Act
        RentalExtra? selectedRentalExtra = await this.rentalExtraRepository.GetByIdAsync(RentalExtra.Id);

        // Assert
        Assert.IsNotNull(selectedRentalExtra);
        Assert.AreEqual(RentalExtra, selectedRentalExtra);
    }
}
