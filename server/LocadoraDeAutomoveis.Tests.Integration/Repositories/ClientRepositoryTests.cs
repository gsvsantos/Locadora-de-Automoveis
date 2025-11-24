using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Drivers;

namespace LocadoraDeAutomoveis.Tests.Integration.Repositories;

[TestClass]
[TestCategory("ClientRepository Infrastructure - Integration Tests")]
public sealed class ClientRepositoryTests : TestFixture
{
    [TestMethod]
    public async Task Should_GetAllAsync_ReturnsClients_Successfully()
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
            Client physicalClient = existingCPFClients[i];

            driver.AssociateTenant(tenant.Id);
            driver.AssociateUser(userEmployee);
            driver.AssociateClientCPF(physicalClient);
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
        List<Client> clients = await this.clientRepository.GetAllAsync();

        // Assert
        Assert.AreEqual(12, clients.Count);
        Assert.AreEqual(8, clients.Count(c => !c.IsJuridical));
        Assert.AreEqual(4, clients.Count(c => c.IsJuridical));

        List<Guid> expectedClientIds = existingCPFClients
            .Concat(existingCNPJClients)
            .Select(c => c.Id)
            .ToList();

        CollectionAssert.AreEquivalent(
            expectedClientIds,
            clients.Select(c => c.Id).ToList()
        );

        foreach (Client client in clients)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(client.Document));
        }

        Client cpfClient0 = clients.First(c => c.Id == existingCPFClients[0].Id);
        Assert.IsFalse(cpfClient0.IsJuridical);

        Client cnpjClient0 = clients.First(c => c.Id == existingCNPJClients[0].Id);
        Assert.IsTrue(cnpjClient0.IsJuridical);
    }

    [TestMethod]
    public async Task Should_GetFiveAsync_ReturnsClients_Successfully()
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
            Client physicalClient = existingCPFClients[i];

            driver.AssociateTenant(tenant.Id);
            driver.AssociateUser(userEmployee);
            driver.AssociateClientCPF(physicalClient);
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
        List<Client> clients = await this.clientRepository.GetAllAsync(5);

        // Assert
        Assert.AreEqual(5, clients.Count);

        HashSet<Guid> expectedClientIds = existingCPFClients
            .Concat(existingCNPJClients)
            .Select(c => c.Id)
            .ToHashSet();

        foreach (Client client in clients)
        {
            Assert.IsTrue(expectedClientIds.Contains(client.Id));
            Assert.IsFalse(string.IsNullOrWhiteSpace(client.Document));
        }
    }

    [TestMethod]
    public async Task Should_GetClientById_ReturnsPhysicalClient_Successfully()
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

        // Act
        Client? selectedClient = await this.clientRepository.GetByIdAsync(existingCPFClient.Id);

        // Assert
        Assert.IsNotNull(selectedClient);
        Assert.AreEqual(existingCPFClient.Id, selectedClient.Id);
        Assert.IsFalse(selectedClient.IsJuridical);
        Assert.AreEqual(existingCPFClient.Document, selectedClient.Document);
    }

    [TestMethod]
    public async Task Should_GetClientById_ReturnsJuridicalClient_Successfully()
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

        Client existingCNPJClient = Builder<Client>.CreateNew()
            .Do(c => c.Document = Guid.NewGuid().ToString()[..14])
            .Build();

        existingCNPJClient.AssociateTenant(tenant.Id);
        existingCNPJClient.AssociateUser(userEmployee);
        existingCNPJClient.MarkAsJuridical();

        await this.clientRepository.AddAsync(existingCNPJClient);
        await this.dbContext.SaveChangesAsync();

        // Act
        Client? selectedClient = await this.clientRepository.GetByIdAsync(existingCNPJClient.Id);

        // Assert
        Assert.IsNotNull(selectedClient);
        Assert.AreEqual(existingCNPJClient.Id, selectedClient.Id);
        Assert.IsTrue(selectedClient.IsJuridical);
        Assert.AreEqual(existingCNPJClient.Document, selectedClient.Document);
    }
}
