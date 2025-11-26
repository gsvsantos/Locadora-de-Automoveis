using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Domain.Clients;

namespace LocadoraDeAutomoveis.Tests.Unit.Clients.Domain;

[TestClass]
[TestCategory("Employee Domain - Unit Tests")]
public sealed class ClientTests
{
    [TestMethod]
    public void ClientConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        Client client = new();

        // Assert
        Assert.AreNotEqual(Guid.Empty, client.Id);
        Assert.IsTrue(client.IsActive);
        Assert.AreEqual(string.Empty, client.FullName);
        Assert.AreEqual(string.Empty, client.Email);
        Assert.AreEqual(string.Empty, client.PhoneNumber);
        Assert.IsTrue(client.ClientType == EClientType.Physical);
        Assert.IsNull(client.Address);
        Assert.AreEqual(string.Empty, client.Document);
        Assert.IsNull(client.LicenseNumber);
    }

    [TestMethod]
    public void ClientConstruct_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        Client client = new(
            "Ricardo",
            "ricardo@gmail.com",
            "(51) 90000-0001",
            "000.000.000-01",
            new(
            "RS",
            "Carazinho",
            "Marcondes",
            "Edi Marcondes",
            33)
        );

        // Assert
        Assert.AreNotEqual(Guid.Empty, client.Id);
        Assert.IsTrue(client.IsActive);
        Assert.AreEqual("Ricardo", client.FullName);
        Assert.AreEqual("ricardo@gmail.com", client.Email);
        Assert.AreEqual("(51) 90000-0001", client.PhoneNumber);
        Assert.IsFalse(client.ClientType == EClientType.Juristic);
        Assert.AreEqual("RS", client.Address.State);
        Assert.AreEqual("Carazinho", client.Address.City);
        Assert.AreEqual("Marcondes", client.Address.Neighborhood);
        Assert.AreEqual("Edi Marcondes", client.Address.Street);
        Assert.AreEqual(33, client.Address.Number);
        Assert.AreEqual("000.000.000-01", client.Document);
        Assert.IsNull(client.LicenseNumber);
    }

    [TestMethod]
    public void ClientMethod_Update_ShouldWorks()
    {
        // Arrange
        Client client = new(
            "Ricardo",
            "ricardo@gmail.com",
            "(51) 90000-0001",
            "000.000.000-01",
            new(
            "RS",
            "Carazinho",
            "Marcondes",
            "Edi Marcondes",
            33)
        );

        Client client2 = new(
            "Ricardo ED",
            "ricardoED@gmail.com",
            "(51) 90000-0002",
            "000.000.000-02",
            new(
            "SA",
            "CarazinhoED",
            "MarcondesED",
            "Edi MarcondesED",
            11)
        );

        // Act
        client.Update(client2);

        // Assert
        Assert.AreNotEqual(Guid.Empty, client.Id);
        Assert.IsTrue(client.IsActive);
        Assert.AreEqual("Ricardo ED", client.FullName);
        Assert.AreEqual("ricardoED@gmail.com", client.Email);
        Assert.AreEqual("(51) 90000-0002", client.PhoneNumber);
        Assert.IsFalse(client.ClientType == EClientType.Juristic);
        Assert.AreEqual("SA", client.Address.State);
        Assert.AreEqual("CarazinhoED", client.Address.City);
        Assert.AreEqual("MarcondesED", client.Address.Neighborhood);
        Assert.AreEqual("Edi MarcondesED", client.Address.Street);
        Assert.AreEqual(11, client.Address.Number);
        Assert.AreEqual("000.000.000-02", client.Document);
        Assert.IsNull(client.LicenseNumber);
    }

    [TestMethod]
    public void ClientMethod_MarkAsJuridical_ShouldWorks()
    {
        // Arrange
        Client client = Builder<Client>.CreateNew().Build();

        // Act
        client.MarkAsJuridical();

        // Assert
        Assert.IsTrue(client.ClientType == EClientType.Juristic);
    }

    [TestMethod]
    public void ClientMethod_MarkAsPhysical_ShouldWorks()
    {
        // Arrange
        Client client = Builder<Client>.CreateNew().Build();

        // Act
        client.MarkAsPhysical();

        // Assert
        Assert.IsTrue(client.ClientType == EClientType.Physical);
    }
}
