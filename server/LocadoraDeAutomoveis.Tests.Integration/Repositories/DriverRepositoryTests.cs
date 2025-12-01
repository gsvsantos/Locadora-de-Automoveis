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

        List<Client> existingCPFClients = Builder<Client>.CreateListOfSize(8).All()
            .Do(c => c.Address = new Address(
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.Int()
            ))
            .Do(c => c.Document = Guid.NewGuid().ToString()[..11])
            .Do(c =>
            {
                c.JuristicClientId = null;
                c.JuristicClient = null;
            })
            .Build().ToList();

        foreach (Client client in existingCPFClients)
        {
            client.AssociateTenant(tenant.Id);
            client.AssociateUser(userEmployee);
            client.SetClientType(EClientType.Individual);
        }

        await this.clientRepository.AddMultiplyAsync(existingCPFClients);

        List<Client> existingCNPJClients = Builder<Client>.CreateListOfSize(4).All()
            .Do(c => c.Address = new Address(
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.Int()
            ))
            .Do(c => c.Document = Guid.NewGuid().ToString()[..14])
            .Do(c =>
            {
                c.JuristicClientId = null;
                c.JuristicClient = null;
            })
            .Build().ToList();

        foreach (Client client in existingCNPJClients)
        {
            client.AssociateTenant(tenant.Id);
            client.AssociateUser(userEmployee);
            client.SetClientType(EClientType.Business);
        }

        foreach (Client client in existingCPFClients.Concat(existingCNPJClients))
        {
            client.JuristicClientId = null;
            client.JuristicClient = null;
        }

        await this.clientRepository.AddMultiplyAsync(existingCNPJClients);

        await this.dbContext.SaveChangesAsync();

        List<Driver> driversCPFClients = Builder<Driver>.CreateListOfSize(4)
            .All()
            .Do(d =>
            {
                d.Document = $"DRV-Individual-{Guid.NewGuid()}";
                d.LicenseNumber = $"LIC-Individual-{Guid.NewGuid()}";
                d.FullName = $"Driver Individual {Guid.NewGuid()}";
                d.Email = $"cpf-{Guid.NewGuid()}@test.com";
                d.LicenseValidity = DateTimeOffset.UtcNow.AddYears(2);
            })
            .Build().ToList();

        for (int i = 0; i < driversCPFClients.Count; i++)
        {
            Driver driver = driversCPFClients[i];
            Client client = existingCPFClients[i];

            driver.AssociateTenant(tenant.Id);
            driver.AssociateUser(userEmployee);
            driver.AssociateClient(client);
        }

        await this.driverRepository.AddMultiplyAsync(driversCPFClients);

        List<Driver> driversCNPJClients = Builder<Driver>.CreateListOfSize(4)
            .All()
            .Do(d =>
            {
                d.Document = $"DRV-Business-{Guid.NewGuid()}";
                d.LicenseNumber = $"LIC-Business-{Guid.NewGuid()}";
                d.FullName = $"Driver Business {Guid.NewGuid()}";
                d.Email = $"cnpj-{Guid.NewGuid()}@test.com";
                d.LicenseValidity = DateTimeOffset.UtcNow.AddYears(2);
            })
            .Build().ToList();

        for (int i = 0; i < driversCNPJClients.Count; i++)
        {
            Driver driver = driversCNPJClients[i];
            Client physicalClient = existingCPFClients[i + driversCPFClients.Count];

            driver.AssociateTenant(tenant.Id);
            driver.AssociateUser(userEmployee);
            driver.AssociateClient(physicalClient);
        }

        await this.driverRepository.AddMultiplyAsync(driversCNPJClients);

        await this.dbContext.SaveChangesAsync();

        // Act 
        List<Driver> drivers = await this.driverRepository.GetAllAsync();

        // Assert
        Assert.AreEqual(8, drivers.Count);
        Assert.AreEqual(8, drivers.Count(d => !d.Client.ClientType.Equals(EClientType.Business)));

        foreach (Driver driver in drivers)
        {
            Assert.AreEqual(driver.ClientId, driver.Client.Id);
            Assert.IsFalse(driver.Client.ClientType == EClientType.Business);
        }

        Driver driver1 = drivers.First(d => d.Id == driversCPFClients[0].Id);
        Assert.AreEqual(existingCPFClients[0].Id, driver1.Client.Id);
        Assert.AreEqual(existingCPFClients[0].FullName, driver1.Client.FullName);

        Driver driver2 = drivers.First(d => d.Id == driversCNPJClients[0].Id);
        Client expectedClientForDriver2 = existingCPFClients[driversCPFClients.Count];
        Assert.AreEqual(expectedClientForDriver2.Id, driver2.Client.Id);
        Assert.AreEqual(expectedClientForDriver2.FullName, driver2.Client.FullName);
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

        List<Client> existingCPFClients = Builder<Client>.CreateListOfSize(8).All()
            .Do(c => c.Address = new Address(
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.Int()
            ))
            .Do(c => c.Document = Guid.NewGuid().ToString()[..11])
            .Do(c =>
            {
                c.JuristicClientId = null;
                c.JuristicClient = null;
            })
            .Build().ToList();

        List<Client> existingCNPJClients = Builder<Client>.CreateListOfSize(4).All()
            .Do(c => c.Address = new Address(
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.Int()
            ))
            .Do(c => c.Document = Guid.NewGuid().ToString()[..14])
            .Do(c =>
            {
                c.JuristicClientId = null;
                c.JuristicClient = null;
            })
            .Build().ToList();

        foreach (Client client in existingCPFClients.Concat(existingCNPJClients))
        {
            client.JuristicClientId = null;
            client.JuristicClient = null;
        }

        foreach (Client juridicalClient in existingCNPJClients)
        {
            juridicalClient.AssociateTenant(tenant.Id);
            juridicalClient.AssociateUser(userEmployee);
            juridicalClient.SetClientType(EClientType.Business);
        }

        foreach (Client physicalClient in existingCPFClients)
        {
            physicalClient.AssociateTenant(tenant.Id);
            physicalClient.AssociateUser(userEmployee);
            physicalClient.SetClientType(EClientType.Individual);
        }

        List<Client> existingClients = existingCNPJClients
             .Concat(existingCPFClients)
             .ToList();

        await this.clientRepository.AddMultiplyAsync(existingClients);

        await this.dbContext.SaveChangesAsync();

        for (int i = 0; i < existingCPFClients.Count; i++)
        {
            Client physicalClient = existingCPFClients[i];

            int juridicalIndex = i / 2;
            Client juridicalClient = existingCNPJClients[juridicalIndex];

            physicalClient.AssociateJuristicClient(juridicalClient);
        }

        await this.dbContext.SaveChangesAsync();

        List<Driver> driversCPFClients = Builder<Driver>.CreateListOfSize(4)
            .All()
            .Do(d =>
            {
                d.Document = $"DRV-Individual-{Guid.NewGuid()}";
                d.LicenseNumber = $"LIC-Individual-{Guid.NewGuid()}";
                d.FullName = $"Driver Individual {Guid.NewGuid()}";
                d.Email = $"cpf-{Guid.NewGuid()}@test.com";
                d.LicenseValidity = DateTimeOffset.UtcNow.AddYears(2);
            })
            .Build().ToList();

        for (int i = 0; i < driversCPFClients.Count; i++)
        {
            Driver driver = driversCPFClients[i];
            Client client = existingCPFClients[i];

            driver.AssociateTenant(tenant.Id);
            driver.AssociateUser(userEmployee);
            driver.AssociateClient(client);
        }

        await this.driverRepository.AddMultiplyAsync(driversCPFClients);

        List<Driver> driversCNPJClients = Builder<Driver>.CreateListOfSize(4)
            .All()
            .Do(d =>
            {
                d.Document = $"DRV-Business-{Guid.NewGuid()}";
                d.LicenseNumber = $"LIC-Business-{Guid.NewGuid()}";
                d.FullName = $"Driver Business {Guid.NewGuid()}";
                d.Email = $"cnpj-{Guid.NewGuid()}@test.com";
                d.LicenseValidity = DateTimeOffset.UtcNow.AddYears(2);
            })
            .Build().ToList();

        for (int i = 0; i < driversCNPJClients.Count; i++)
        {
            Driver driver = driversCNPJClients[i];
            Client physicalClient = existingCPFClients[i + driversCPFClients.Count];

            driver.AssociateTenant(tenant.Id);
            driver.AssociateUser(userEmployee);
            driver.AssociateClient(physicalClient);
        }

        await this.driverRepository.AddMultiplyAsync(driversCNPJClients);

        await this.dbContext.SaveChangesAsync();

        // Act 
        List<Driver> drivers = await this.driverRepository.GetAllAsync(5);

        // Assert
        Assert.AreEqual(5, drivers.Count);

        foreach (Driver driver in drivers)
        {
            Assert.AreEqual(driver.ClientId, driver.Client.Id);
            Assert.IsFalse(driver.Client.ClientType == EClientType.Business);
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
            .Do(c => c.Address = new Address(
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.Int()
            ))
            .Do(c => c.Document = Guid.NewGuid().ToString()[..11])
            .Do(c =>
            {
                c.JuristicClientId = null;
                c.JuristicClient = null;
            })
            .Build();

        existingCPFClient.AssociateTenant(tenant.Id);
        existingCPFClient.AssociateUser(userEmployee);
        existingCPFClient.SetClientType(EClientType.Individual);

        await this.clientRepository.AddAsync(existingCPFClient);

        await this.dbContext.SaveChangesAsync();

        Driver driver = Builder<Driver>.CreateNew()
            .Do(d =>
            {
                d.Document = $"DRV-Individual-{Guid.NewGuid()}";
                d.LicenseNumber = $"LIC-Individual-{Guid.NewGuid()}";
                d.FullName = $"Driver Individual {Guid.NewGuid()}";
                d.Email = $"cpf-{Guid.NewGuid()}@test.com";
                d.LicenseValidity = DateTimeOffset.UtcNow.AddYears(2);
            })
            .Build();

        driver.AssociateTenant(tenant.Id);
        driver.AssociateUser(userEmployee);
        driver.AssociateClient(existingCPFClient);

        await this.driverRepository.AddAsync(driver);

        await this.dbContext.SaveChangesAsync();

        // Act 
        Driver? selectedDriver = await this.driverRepository.GetByIdAsync(driver.Id);

        // Assert
        Assert.IsNotNull(selectedDriver);
        Assert.AreEqual(existingCPFClient.Id, selectedDriver.Client.Id);
        Assert.AreEqual(existingCPFClient.FullName, selectedDriver.Client.FullName);
        Assert.AreEqual(EClientType.Individual, selectedDriver.Client.ClientType);
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
            .Do(c => c.Address = new Address(
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.Int()
            ))
            .Do(c => c.Document = Guid.NewGuid().ToString()[..11])
            .Do(c =>
            {
                c.JuristicClientId = null;
                c.JuristicClient = null;
            })
            .Build();

        existingCPFClient.AssociateTenant(tenant.Id);
        existingCPFClient.AssociateUser(userEmployee);
        existingCPFClient.SetClientType(EClientType.Individual);

        await this.clientRepository.AddAsync(existingCPFClient);

        Client existingClientCNPJ = Builder<Client>.CreateNew()
            .Do(c => c.Address = new Address(
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.Int()
            ))
            .Do(c => c.Document = Guid.NewGuid().ToString()[..14])
            .Do(c =>
            {
                c.JuristicClientId = null;
                c.JuristicClient = null;
            })
            .Build();

        existingClientCNPJ.AssociateTenant(tenant.Id);
        existingClientCNPJ.AssociateUser(userEmployee);
        existingClientCNPJ.SetClientType(EClientType.Business);

        await this.clientRepository.AddAsync(existingClientCNPJ);

        await this.dbContext.SaveChangesAsync();

        existingCPFClient.AssociateJuristicClient(existingClientCNPJ);

        await this.dbContext.SaveChangesAsync();

        Driver driver = Builder<Driver>.CreateNew()
            .Do(d =>
            {
                d.Document = $"DRV-Individual-{Guid.NewGuid()}";
                d.LicenseNumber = $"LIC-Individual-{Guid.NewGuid()}";
                d.FullName = $"Driver Individual {Guid.NewGuid()}";
                d.Email = $"cpf-{Guid.NewGuid()}@test.com";
                d.LicenseValidity = DateTimeOffset.UtcNow.AddYears(2);
            })
            .Build();

        driver.AssociateTenant(tenant.Id);
        driver.AssociateUser(userEmployee);
        driver.AssociateClient(existingCPFClient);

        await this.driverRepository.AddAsync(driver);

        await this.dbContext.SaveChangesAsync();

        // Act 
        Driver? selectedDriver = await this.driverRepository.GetByIdAsync(driver.Id);

        // Assert
        Assert.IsNotNull(selectedDriver);
        Assert.AreEqual(existingCPFClient.Id, selectedDriver.Client.Id);
        Assert.AreEqual(existingCPFClient.FullName, selectedDriver.Client.FullName);
        Assert.AreEqual(EClientType.Individual, selectedDriver.Client.ClientType);

        Assert.IsNotNull(selectedDriver.Client.JuristicClient);
        Assert.AreEqual(existingClientCNPJ.Id, selectedDriver.Client.JuristicClient.Id);
        Assert.AreEqual(existingClientCNPJ.FullName, selectedDriver.Client.JuristicClient.FullName);
    }
}
