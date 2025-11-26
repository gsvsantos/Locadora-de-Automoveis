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
    public void DriverConstructor_Parametered_ShouldInitializeProperties()
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
    public void VehicleMethod_Update_ShouldWorks()
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
    public void VehicleMethod_AssociateClientCPF_ShouldWorks()
    {
        // Arrange
        Driver driver = new();

        Client client = Builder<Client>.CreateNew().Build();
        client.MarkAsPhysical();

        // Act
        driver.AssociateClient(client);

        // Assert
        Assert.AreEqual(client.Id, driver.ClientId);
        Assert.AreEqual(client, driver.Client);
        Assert.IsTrue(driver.Client.ClientType == EClientType.Physical);
    }

    [TestMethod]
    public void VehicleMethod_DisassociateClientCPF_ShouldWorks()
    {
        // Arrange
        Driver driver = new();

        Client client = Builder<Client>.CreateNew().Build();
        client.MarkAsPhysical();

        driver.AssociateClient(client);

        // Act
        driver.DisassociateClient();

        // Assert
        Assert.AreNotEqual(client.Id, driver.ClientId);
        Assert.AreNotEqual(client, driver.Client);
    }

    [TestMethod]
    public void VehicleMethod_AssociateClientCNPJ_ShouldWorks()
    {
        // Arrange
        Driver driver = new();

        Client clientCNPJ = Builder<Client>.CreateNew().Build();
        clientCNPJ.MarkAsJuridical();

        // Act
        driver.AssociateClient(clientCNPJ);

        // Assert
        Assert.AreEqual(clientCNPJ.Id, driver.ClientId);
        Assert.AreEqual(clientCNPJ, driver.Client);
        Assert.IsTrue(driver.Client.ClientType == EClientType.Juristic);
    }

    [TestMethod]
    public void VehicleMethod_DisassociateClientCNPJ_ShouldWorks()
    {
        // Arrange
        Driver driver = new();

        Client clientCNPJ = Builder<Client>.CreateNew().Build();
        clientCNPJ.MarkAsJuridical();

        driver.AssociateClient(clientCNPJ);

        // Act
        driver.DisassociateClient();

        // Assert
        Assert.AreNotEqual(clientCNPJ.Id, driver.ClientId);
        Assert.AreNotEqual(clientCNPJ, driver.Client);
    }
}
