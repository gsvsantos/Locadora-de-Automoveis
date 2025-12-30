using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Drivers;

namespace LocadoraDeAutomoveis.Tests.Unit.Drivers.Domain;

[TestClass]
[TestCategory("Driver Domain - Unit Tests")]
public sealed class DriverTests
{
    [TestMethod]
    public void DriverConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        Driver driver = new();

        // Assert
        Assert.AreNotEqual(Guid.Empty, driver.Id);
        Assert.IsTrue(driver.IsActive);
        Assert.AreEqual(string.Empty, driver.FullName);
        Assert.AreEqual(string.Empty, driver.Email);
        Assert.AreEqual(string.Empty, driver.PhoneNumber);
        Assert.AreEqual(string.Empty, driver.Document);
        Assert.AreEqual(string.Empty, driver.LicenseNumber);
        Assert.AreEqual(DateTimeOffset.MinValue, driver.LicenseValidity);
    }

    [TestMethod]
    public void DriverConstructor_Parametered_ShouldWorks()
    {
        // Arrange & Act
        DateTimeOffset date = DateTimeOffset.Now;
        Driver driver = new(
            "João Motorista",
            "joao.mot@email.com",
            "(51) 90009-9999",
            "111.111.111-11",
            "12345",
            date
        );

        // Assert
        Assert.AreNotEqual(Guid.Empty, driver.Id);
        Assert.IsTrue(driver.IsActive);
        Assert.AreEqual("João Motorista", driver.FullName);
        Assert.AreEqual("joao.mot@email.com", driver.Email);
        Assert.AreEqual("(51) 90009-9999", driver.PhoneNumber);
        Assert.AreEqual("111.111.111-11", driver.Document);
        Assert.AreEqual("12345", driver.LicenseNumber);
        Assert.AreEqual(date, driver.LicenseValidity);
    }

    [TestMethod]
    public void DriverMethod_Update_ShouldWorks()
    {
        RandomGenerator random = new();

        // Arrange 
        DateTimeOffset date = DateTimeOffset.Now;
        Driver driver = new(
            "João Motorista",
            "joao.mot@email.com",
            "(51) 90009-9999",
            "111.111.111-11",
            "12345",
            random.DateTime().ToLocalTime()
        );

        Driver updatedDriver = new(
            "João Motorista ED",
            "joao.motED@email.com",
            "(51) 99999-0000",
            "222.222.222-22",
            "54321",
            date
        );

        // Act
        driver.Update(updatedDriver);

        // Assert
        Assert.AreNotEqual(Guid.Empty, driver.Id);
        Assert.IsTrue(driver.IsActive);
        Assert.AreEqual("João Motorista ED", driver.FullName);
        Assert.AreEqual("joao.motED@email.com", driver.Email);
        Assert.AreEqual("(51) 99999-0000", driver.PhoneNumber);
        Assert.AreEqual("222.222.222-22", driver.Document);
        Assert.AreEqual("54321", driver.LicenseNumber);
        Assert.AreEqual(date, driver.LicenseValidity);
    }

    [TestMethod]
    public void DriverMethod_AssociateClientCPF_ShouldWorks()
    {
        // Arrange
        Driver driver = new();

        Client client = Builder<Client>.CreateNew().Build();
        client.DefineType(EClientType.Individual);

        // Act
        driver.AssociateClient(client);

        // Assert
        Assert.AreEqual(client.Id, driver.ClientId);
        Assert.AreEqual(client, driver.Client);
        Assert.IsTrue(driver.Client.Type == EClientType.Individual);
    }

    [TestMethod]
    public void DriverMethod_DisassociateClientCPF_ShouldWorks()
    {
        // Arrange
        Driver driver = new();

        Client client = Builder<Client>.CreateNew().Build();
        client.DefineType(EClientType.Individual);

        driver.AssociateClient(client);

        // Act
        driver.DisassociateClient();

        // Assert
        Assert.AreNotEqual(client.Id, driver.ClientId);
        Assert.AreNotEqual(client, driver.Client);
    }

    [TestMethod]
    public void DriverMethod_AssociateClientCNPJ_ShouldWorks()
    {
        // Arrange
        Driver driver = new();

        Client clientCNPJ = Builder<Client>.CreateNew().Build();
        clientCNPJ.DefineType(EClientType.Business);

        // Act
        driver.AssociateClient(clientCNPJ);

        // Assert
        Assert.AreEqual(clientCNPJ.Id, driver.ClientId);
        Assert.AreEqual(clientCNPJ, driver.Client);
        Assert.IsTrue(driver.Client.Type == EClientType.Business);
    }

    [TestMethod]
    public void DriverMethod_DisassociateClientCNPJ_ShouldWorks()
    {
        // Arrange
        Driver driver = new();

        Client clientCNPJ = Builder<Client>.CreateNew().Build();
        clientCNPJ.DefineType(EClientType.Business);

        driver.AssociateClient(clientCNPJ);

        // Act
        driver.DisassociateClient();

        // Assert
        Assert.AreNotEqual(clientCNPJ.Id, driver.ClientId);
        Assert.AreNotEqual(clientCNPJ, driver.Client);
    }

    [TestMethod]
    public void DriverMethod_AssociateClient_ShouldReturn()
    {
        // Arrange
        Driver driver = new();

        Client client = Builder<Client>.CreateNew().Build();
        client.DefineType(EClientType.Individual);
        driver.AssociateClient(client);

        // Act
        driver.AssociateClient(client);

        // Assert
        Assert.AreEqual(client.Id, driver.ClientId);
        Assert.AreEqual(client, driver.Client);
        Assert.IsTrue(driver.Client.Type == EClientType.Individual);
    }

    [TestMethod]
    public void DriverMethod_AssociateClient_ShouldDisassociateBefore_And_Work()
    {
        // Arrange
        Driver driver = new();

        Client client1 = Builder<Client>.CreateNew().Build();
        client1.DefineType(EClientType.Business);
        driver.AssociateClient(client1);

        Client client2 = Builder<Client>.CreateNew().Build();
        client2.DefineType(EClientType.Individual);

        // Act
        driver.AssociateClient(client2);

        // Assert
        Assert.AreEqual(client2.Id, driver.ClientId);
        Assert.AreEqual(client2, driver.Client);
        Assert.IsTrue(driver.Client.Type == EClientType.Individual);
    }

    [TestMethod]
    public void DriverMethod_DisassociateClient_ShouldNotWork()
    {
        // Arrange
        Driver driver = new();

        // Act
        driver.DisassociateClient();

        // Assert
        Assert.AreEqual(Guid.Empty, driver.ClientId);
        Assert.IsNull(driver.Client);
    }
}
