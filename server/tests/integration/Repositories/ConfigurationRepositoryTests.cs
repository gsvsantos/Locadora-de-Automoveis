using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Configurations;

namespace LocadoraDeAutomoveis.Tests.Integration.Repositories;

[TestClass]
[TestCategory("ConfigurationRepository Infrastructure - Integration Tests")]
public sealed class ConfigurationRepositoryTests : TestFixture
{
    [TestMethod]
    public async Task Should_GetByTenantId_ReturnConfiguration_Successfuly()
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

        Configuration configuration = Builder<Configuration>.CreateNew().Build();
        configuration.AssociateTenant(tenant.Id);
        configuration.AssociateUser(userEmployee);

        await this.configurationRepository.AddAsync(configuration);

        await this.dbContext.SaveChangesAsync();

        // Act
        Configuration? selectedConfiguration = await this.configurationRepository.GetByTenantId(tenant.Id);

        // Assert
        Assert.IsNotNull(selectedConfiguration);
        Assert.AreEqual(configuration.Id, selectedConfiguration.Id);
        Assert.AreEqual(tenant.Id, selectedConfiguration.TenantId);
        Assert.AreEqual(configuration.GasolinePrice, selectedConfiguration.GasolinePrice);
    }

    [TestMethod]
    public async Task Should_GetByIdAsync_ReturnConfiguration_Successfuly()
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

        Configuration configuration = Builder<Configuration>.CreateNew().Build();
        configuration.AssociateTenant(tenant.Id);
        configuration.AssociateUser(userEmployee);

        await this.configurationRepository.AddAsync(configuration);

        await this.dbContext.SaveChangesAsync();

        // Act
        Configuration? selectedConfiguration = await this.configurationRepository.GetByIdAsync(configuration.Id);

        // Assert
        Assert.IsNotNull(selectedConfiguration);
        Assert.AreEqual(configuration, selectedConfiguration);
        Assert.AreEqual(configuration.GasolinePrice, selectedConfiguration.GasolinePrice);
    }
}