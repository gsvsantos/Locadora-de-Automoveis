using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Drivers;

namespace LocadoraDeAutomoveis.Tests.Integration.Repositories;

[TestClass]
[TestCategory("DriverRepository Infrastructure - Integration Tests")]
public sealed class DriverRepositoryTests : TestFixture
{
    [TestMethod]
    public async Task Should_GetAllAsync_ReturnsDriverWithClients_Successfully()
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

        List<Client> existingCPFClients = Builder<Client>.CreateListOfSize(8)
                .All()
                .Do(c => c.Document = Guid.NewGuid().ToString()[..11])
                .Build().ToList();
        foreach (Client client in existingCPFClients)
        {
            client.AssociateTenant(tenant.Id);
            client.AssociateUser(userEmployee);
            client.MarkAsPhysical();
        }
        await this.clientRepository.AddMultiplyAsync(existingCPFClients);

        List<Client> existingCNPJClients = Builder<Client>.CreateListOfSize(4)
                .All()
                .Do(c => c.Document = Guid.NewGuid().ToString()[..14])
                .Build().ToList();
        foreach (Client client in existingCNPJClients)
        {
            client.AssociateTenant(tenant.Id);
            client.AssociateUser(userEmployee);
            client.MarkAsJuridical();
        }
        await this.clientRepository.AddMultiplyAsync(existingCNPJClients);

        await this.dbContext.SaveChangesAsync();

        List<Driver> driversCPFClients = Builder<Driver>.CreateListOfSize(4)
            .All()
            .Do(d =>
            {
                d.Document = $"DRV-CPF-{Guid.NewGuid()}";
                d.LicenseNumber = $"LIC-CPF-{Guid.NewGuid()}";
                d.FullName = $"Driver CPF {Guid.NewGuid()}";
                d.Email = $"cpf-{Guid.NewGuid()}@test.com";
                d.LicenseValidity = DateTimeOffset.UtcNow.AddYears(2);
                d.ClientCNPJId = null;
                d.ClientCNPJ = null;
            })
            .Build().ToList();
        for (int i = 0; i < driversCPFClients.Count; i++)
        {
            Driver driver = driversCPFClients[i];
            Client client = existingCPFClients[i];

            driver.AssociateTenant(tenant.Id);
            driver.AssociateUser(userEmployee);
            driver.AssociateClientCPF(client);
        }
        await this.driverRepository.AddMultiplyAsync(driversCPFClients);

        List<Driver> driversCNPJClients = Builder<Driver>.CreateListOfSize(4)
            .All()
            .Do(d =>
            {
                d.Document = $"DRV-CNPJ-{Guid.NewGuid()}";
                d.LicenseNumber = $"LIC-CNPJ-{Guid.NewGuid()}";
                d.FullName = $"Driver CNPJ {Guid.NewGuid()}";
                d.Email = $"cnpj-{Guid.NewGuid()}@test.com";
                d.LicenseValidity = DateTimeOffset.UtcNow.AddYears(2);
            })
            .Build().ToList();
        for (int i = 0; i < driversCNPJClients.Count; i++)
        {
            Driver driver = driversCNPJClients[i];
            Client juridicalClient = existingCNPJClients[i];
            Client physicalClient = existingCPFClients[i + driversCPFClients.Count];

            driver.AssociateTenant(tenant.Id);
            driver.AssociateUser(userEmployee);
            driver.AssociateClientCPF(physicalClient);
            driver.AssociateClientCNPJ(juridicalClient);
        }
        await this.driverRepository.AddMultiplyAsync(driversCNPJClients);

        await this.dbContext.SaveChangesAsync();

        // Act 
        List<Driver> drivers = await this.driverRepository.GetAllAsync();

        // Assert
        Assert.AreEqual(8, drivers.Count);
        Assert.AreEqual(4, drivers.Count(d => d.ClientCNPJ is null));
        Assert.AreEqual(4, drivers.Count(d => d.ClientCNPJ is not null));

        foreach (Driver driver in drivers)
        {
            Assert.AreEqual(driver.ClientCPFId, driver.ClientCPF.Id);
            Assert.IsFalse(driver.ClientCPF.IsJuridical);
        }

        foreach (Driver driver in drivers.Where(d => d.ClientCNPJ is not null))
        {
            Assert.AreEqual(driver.ClientCNPJId, driver.ClientCNPJ!.Id);
            Assert.IsTrue(driver.ClientCNPJ.IsJuridical);
        }

        Driver driver1 = drivers.First(d => d.Id == driversCPFClients[0].Id);
        Assert.AreEqual(existingCPFClients[0].Id, driver1.ClientCPF.Id);
        Assert.AreEqual(existingCPFClients[0].FullName, driver1.ClientCPF.FullName);

        Driver driver2 = drivers.First(d => d.Id == driversCNPJClients[0].Id);
        Assert.IsNotNull(driver2.ClientCNPJ);
        Assert.AreEqual(existingCNPJClients[0].Id, driver2.ClientCNPJ.Id);
        Assert.AreEqual(existingCNPJClients[0].FullName, driver2.ClientCNPJ.FullName);
    }

    [TestMethod]
    public async Task Should_GetFiveAsync_ReturnsDriverWithClients_Successfully()
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

        List<Client> existingCPFClients = Builder<Client>.CreateListOfSize(8)
                .All()
                .Do(c => c.Document = Guid.NewGuid().ToString()[..11])
                .Build().ToList();
        foreach (Client client in existingCPFClients)
        {
            client.AssociateTenant(tenant.Id);
            client.AssociateUser(userEmployee);
            client.MarkAsPhysical();
        }
        await this.clientRepository.AddMultiplyAsync(existingCPFClients);

        List<Client> existingCNPJClients = Builder<Client>.CreateListOfSize(4)
                .All()
                .Do(c => c.Document = Guid.NewGuid().ToString()[..14])
                .Build().ToList();
        foreach (Client client in existingCNPJClients)
        {
            client.AssociateTenant(tenant.Id);
            client.AssociateUser(userEmployee);
            client.MarkAsJuridical();
        }
        await this.clientRepository.AddMultiplyAsync(existingCNPJClients);

        await this.dbContext.SaveChangesAsync();

        List<Driver> driversCPFClients = Builder<Driver>.CreateListOfSize(4)
            .All()
            .Do(d =>
            {
                d.Document = $"DRV-CPF-{Guid.NewGuid()}";
                d.LicenseNumber = $"LIC-CPF-{Guid.NewGuid()}";
                d.FullName = $"Driver CPF {Guid.NewGuid()}";
                d.Email = $"cpf-{Guid.NewGuid()}@test.com";
                d.LicenseValidity = DateTimeOffset.UtcNow.AddYears(2);
                d.ClientCNPJId = null;
                d.ClientCNPJ = null;
            })
            .Build().ToList();
        for (int i = 0; i < driversCPFClients.Count; i++)
        {
            Driver driver = driversCPFClients[i];
            Client client = existingCPFClients[i];

            driver.AssociateTenant(tenant.Id);
            driver.AssociateUser(userEmployee);
            driver.AssociateClientCPF(client);
        }
        await this.driverRepository.AddMultiplyAsync(driversCPFClients);

        List<Driver> driversCNPJClients = Builder<Driver>.CreateListOfSize(4)
            .All()
            .Do(d =>
            {
                d.Document = $"DRV-CNPJ-{Guid.NewGuid()}";
                d.LicenseNumber = $"LIC-CNPJ-{Guid.NewGuid()}";
                d.FullName = $"Driver CNPJ {Guid.NewGuid()}";
                d.Email = $"cnpj-{Guid.NewGuid()}@test.com";
                d.LicenseValidity = DateTimeOffset.UtcNow.AddYears(2);
            })
            .Build().ToList();
        for (int i = 0; i < driversCNPJClients.Count; i++)
        {
            Driver driver = driversCNPJClients[i];
            Client juridicalClient = existingCNPJClients[i];
            Client physicalClient = existingCPFClients[i + driversCPFClients.Count];

            driver.AssociateTenant(tenant.Id);
            driver.AssociateUser(userEmployee);
            driver.AssociateClientCPF(physicalClient);
            driver.AssociateClientCNPJ(juridicalClient);
        }
        await this.driverRepository.AddMultiplyAsync(driversCNPJClients);

        await this.dbContext.SaveChangesAsync();

        // Act 
        List<Driver> drivers = await this.driverRepository.GetAllAsync(5);

        // Assert
        Assert.AreEqual(5, drivers.Count);

        foreach (Driver driver in drivers)
        {
            Assert.AreEqual(driver.ClientCPFId, driver.ClientCPF.Id);
            Assert.IsFalse(driver.ClientCPF.IsJuridical);

            if (driver.ClientCNPJ is not null)
            {
                Assert.AreEqual(driver.ClientCNPJId, driver.ClientCNPJ.Id);
                Assert.IsTrue(driver.ClientCNPJ.IsJuridical);
            }
            else
            {
                Assert.IsNull(driver.ClientCNPJId);
            }
        }
    }

    [TestMethod]
    public async Task Should_GetDriverById_ReturnsDriverWithCPFClient_Successfully()
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

        Client existingCPFClient = Builder<Client>.CreateNew()
                .Do(c => c.Document = Guid.NewGuid().ToString()[..11])
                .Build();
        existingCPFClient.AssociateTenant(tenant.Id);
        existingCPFClient.AssociateUser(userEmployee);
        existingCPFClient.MarkAsPhysical();

        await this.clientRepository.AddAsync(existingCPFClient);

        await this.dbContext.SaveChangesAsync();

        Driver driver = Builder<Driver>.CreateNew()
            .Do(d =>
            {
                d.Document = $"DRV-CPF-{Guid.NewGuid()}";
                d.LicenseNumber = $"LIC-CPF-{Guid.NewGuid()}";
                d.FullName = $"Driver CPF {Guid.NewGuid()}";
                d.Email = $"cpf-{Guid.NewGuid()}@test.com";
                d.LicenseValidity = DateTimeOffset.UtcNow.AddYears(2);
                d.ClientCNPJId = null;
                d.ClientCNPJ = null;
            })
            .Build();

        driver.AssociateTenant(tenant.Id);
        driver.AssociateUser(userEmployee);
        driver.AssociateClientCPF(existingCPFClient);

        await this.driverRepository.AddAsync(driver);

        await this.dbContext.SaveChangesAsync();

        // Act 
        Driver? selectedDriver = await this.driverRepository.GetByIdAsync(driver.Id);

        // Assert
        Assert.IsNotNull(selectedDriver);
        Assert.AreEqual(existingCPFClient, selectedDriver.ClientCPF);
    }

    [TestMethod]
    public async Task Should_GetDriverById_ReturnsDriverWithClients_Successfully()
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

        Client existingCPFClient = Builder<Client>.CreateNew()
                .Do(c => c.Document = Guid.NewGuid().ToString()[..11])
                .Build();
        existingCPFClient.AssociateTenant(tenant.Id);
        existingCPFClient.AssociateUser(userEmployee);
        existingCPFClient.MarkAsPhysical();

        await this.clientRepository.AddAsync(existingCPFClient);

        Client existingClientCNPJ = Builder<Client>.CreateNew()
                .Do(c => c.Document = Guid.NewGuid().ToString()[..14])
                .Build();
        existingClientCNPJ.AssociateTenant(tenant.Id);
        existingClientCNPJ.AssociateUser(userEmployee);
        existingClientCNPJ.MarkAsJuridical();

        await this.clientRepository.AddAsync(existingClientCNPJ);

        await this.dbContext.SaveChangesAsync();

        Driver driver = Builder<Driver>.CreateNew()
            .Do(d =>
            {
                d.Document = $"DRV-CPF-{Guid.NewGuid()}";
                d.LicenseNumber = $"LIC-CPF-{Guid.NewGuid()}";
                d.FullName = $"Driver CPF {Guid.NewGuid()}";
                d.Email = $"cpf-{Guid.NewGuid()}@test.com";
                d.LicenseValidity = DateTimeOffset.UtcNow.AddYears(2);
                d.ClientCNPJId = null;
                d.ClientCNPJ = null;
            })
            .Build();

        driver.AssociateTenant(tenant.Id);
        driver.AssociateUser(userEmployee);
        driver.AssociateClientCPF(existingCPFClient);
        driver.AssociateClientCNPJ(existingClientCNPJ);

        await this.driverRepository.AddAsync(driver);

        await this.dbContext.SaveChangesAsync();

        // Act 
        Driver? selectedDriver = await this.driverRepository.GetByIdAsync(driver.Id);

        // Assert
        Assert.IsNotNull(selectedDriver);
        Assert.AreEqual(existingCPFClient, selectedDriver.ClientCPF);

        Assert.IsNotNull(selectedDriver);
        Assert.AreEqual(existingCPFClient, selectedDriver.ClientCPF);
        Assert.AreEqual(existingClientCNPJ, selectedDriver.ClientCNPJ);
    }
}
