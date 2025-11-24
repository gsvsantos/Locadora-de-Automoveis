using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.RateServices;

namespace LocadoraDeAutomoveis.Tests.Integration.Repositories;

[TestClass]
[TestCategory("RateServiceRepository Infrastructure - Integration Tests")]
public sealed class RateServiceRepositoryTests : TestFixture
{
    // to-do: testar com include de Aluguél

    [TestMethod]
    public async Task Should_GetAllAsync_ReturnRateServices_Successfuly()
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

        List<RateService> existingRateServices = Builder<RateService>.CreateListOfSize(10).Build().ToList();
        foreach (RateService rateService in existingRateServices)
        {
            rateService.AssociateTenant(tenant.Id);
            rateService.AssociateUser(userEmployee);
        }

        await this.rateServiceRepository.AddMultiplyAsync(existingRateServices);

        await this.dbContext.SaveChangesAsync();

        // Act
        List<RateService> rateServices = await this.rateServiceRepository.GetAllAsync();

        // Assert
        CollectionAssert.AreEquivalent(existingRateServices, rateServices);
        Assert.AreEqual(10, rateServices.Count);
    }

    [TestMethod]
    public async Task Should_GetFiveAsync_ReturnRateServices_Successfuly()
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

        List<RateService> existingRateServices = Builder<RateService>.CreateListOfSize(10).Build().ToList();
        foreach (RateService rateService in existingRateServices)
        {
            rateService.AssociateTenant(tenant.Id);
            rateService.AssociateUser(userEmployee);
        }

        await this.rateServiceRepository.AddMultiplyAsync(existingRateServices);

        await this.dbContext.SaveChangesAsync();

        // Act
        List<RateService> rateServices = await this.rateServiceRepository.GetAllAsync(5);

        // Assert
        Assert.AreEqual(5, rateServices.Count);
    }

    [TestMethod]
    public async Task Should_GetGroupById_ReturnRateService_Successfuly()
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

        RateService rateService = Builder<RateService>.CreateNew().Build();
        rateService.AssociateTenant(tenant.Id);
        rateService.AssociateUser(userEmployee);

        await this.rateServiceRepository.AddAsync(rateService);

        await this.dbContext.SaveChangesAsync();

        // Act
        RateService? selectedRateService = await this.rateServiceRepository.GetByIdAsync(rateService.Id);

        // Assert
        Assert.IsNotNull(selectedRateService);
        Assert.AreEqual(rateService, selectedRateService);
    }
}
