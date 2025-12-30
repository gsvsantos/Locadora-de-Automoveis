using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Domain.Auth;
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
        Assert.IsTrue(client.Type == EClientType.Individual);
        Assert.IsNull(client.Address);
        Assert.IsNull(client.Document);
        Assert.IsNull(client.LicenseNumber);
        Assert.IsNull(client.JuristicClientId);
        Assert.IsNull(client.JuristicClient);
    }

    [TestMethod]
    public void ClientFirstConstructor_Parameterized_ShouldInitializeProperties()
    {
        // Arrange & Act
        Client client = new(
            "Ricardo",
            "ricardo@gmail.com",
            "(51) 90000-0001"
        );

        // Assert
        Assert.AreNotEqual(Guid.Empty, client.Id);
        Assert.IsTrue(client.IsActive);
        Assert.AreEqual("Ricardo", client.FullName);
        Assert.AreEqual("ricardo@gmail.com", client.Email);
        Assert.AreEqual("(51) 90000-0001", client.PhoneNumber);
        Assert.IsFalse(client.Type == EClientType.Business);
        Assert.IsNull(client.Address);
        Assert.IsNull(client.Document);
        Assert.IsNull(client.LicenseNumber);
        Assert.IsNull(client.LicenseValidity);
        Assert.IsNull(client.JuristicClientId);
        Assert.IsNull(client.JuristicClient);
    }

    [TestMethod]
    public void ClientSecondConstructor_Parameterized_ShouldInitializeProperties()
    {
        // Arrange & Act
        Client client = new(
            "Ricardo",
            "ricardo@gmail.com",
            "(51) 90000-0001",
            "000.000.000-01",
            new("RS", "Carazinho", "Marcondes", "Edi Marcondes", 33)
        );

        // Assert
        Assert.AreNotEqual(Guid.Empty, client.Id);
        Assert.IsTrue(client.IsActive);
        Assert.AreEqual("Ricardo", client.FullName);
        Assert.AreEqual("ricardo@gmail.com", client.Email);
        Assert.AreEqual("(51) 90000-0001", client.PhoneNumber);
        Assert.IsFalse(client.Type == EClientType.Business);
        Assert.AreEqual("000.000.000-01", client.Document);
        Assert.IsNotNull(client.Address);
        Assert.AreEqual("RS", client.Address.State);
        Assert.AreEqual("Carazinho", client.Address.City);
        Assert.AreEqual("Marcondes", client.Address.Neighborhood);
        Assert.AreEqual("Edi Marcondes", client.Address.Street);
        Assert.AreEqual(33, client.Address.Number);
        Assert.IsNull(client.LicenseNumber);
        Assert.IsNull(client.LicenseValidity);
        Assert.IsNull(client.JuristicClientId);
        Assert.IsNull(client.JuristicClient);
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
            new("RS", "Carazinho", "Marcondes", "Edi Marcondes", 33)
        );

        Client client2 = new(
            "Ricardo ED",
            "ricardoED@gmail.com",
            "(51) 90000-0002",
            "000.000.000-02",
            new("SA", "CarazinhoED", "MarcondesED", "Edi MarcondesED", 11)
        );
        string cnh = "123456789";
        DateTimeOffset date = new(2026, 05, 20, 0, 0, 0, TimeSpan.Zero);
        client2.SetLicense(cnh, date);

        // Act
        client.Update(client2);

        // Assert
        Assert.AreNotEqual(Guid.Empty, client.Id);
        Assert.IsTrue(client.IsActive);
        Assert.AreEqual("Ricardo ED", client.FullName);
        Assert.AreEqual("ricardoED@gmail.com", client.Email);
        Assert.AreEqual("(51) 90000-0002", client.PhoneNumber);
        Assert.IsFalse(client.Type == EClientType.Business);
        Assert.AreEqual("000.000.000-02", client.Document);
        Assert.IsNotNull(client.Address);
        Assert.AreEqual("SA", client.Address.State);
        Assert.AreEqual("CarazinhoED", client.Address.City);
        Assert.AreEqual("MarcondesED", client.Address.Neighborhood);
        Assert.AreEqual("Edi MarcondesED", client.Address.Street);
        Assert.AreEqual(11, client.Address.Number);
        Assert.AreEqual(cnh, client.LicenseNumber);
        Assert.AreEqual(date, client.LicenseValidity);
    }

    [TestMethod]
    public void ClientMethod_MarkAsJuridical_ShouldWorks()
    {
        // Arrange
        Client client = Builder<Client>.CreateNew().Build();

        // Act
        client.DefineType(EClientType.Business);

        // Assert
        Assert.IsTrue(client.Type == EClientType.Business);
    }

    [TestMethod]
    public void ClientMethod_MarkAsPhysical_ShouldWorks()
    {
        // Arrange
        Client client = Builder<Client>.CreateNew().Build();

        // Act
        client.DefineType(EClientType.Individual);

        // Assert
        Assert.IsTrue(client.Type == EClientType.Individual);
    }

    [TestMethod]
    public void ClientMethod_SetLicenseNumber_ShouldWorks()
    {
        // Arrange
        Client client = Builder<Client>.CreateNew().Build();
        string cnh = "123456789";

        // Act
        client.SetLicenseNumber(cnh);

        // Assert
        Assert.AreEqual(cnh, client.LicenseNumber);
    }

    [TestMethod]
    public void ClientMethod_SetLicenseValidity_ShouldWorks()
    {
        // Arrange
        Client client = Builder<Client>.CreateNew().Build();
        DateTimeOffset date = new(2026, 05, 20, 0, 0, 0, TimeSpan.Zero);

        // Act
        client.SetLicenseValidity(date);

        // Assert
        Assert.AreEqual(date, client.LicenseValidity);
    }

    [TestMethod]
    public void ClientMethod_SetLicense_ShouldWorks()
    {
        // Arrange
        Client client = Builder<Client>.CreateNew().Build();
        string cnh = "123456789";
        DateTimeOffset date = new(2026, 05, 20, 0, 0, 0, TimeSpan.Zero);

        // Act
        client.SetLicense(cnh, date);

        // Assert
        Assert.AreEqual(cnh, client.LicenseNumber);
        Assert.AreEqual(date, client.LicenseValidity);
    }

    [TestMethod]
    public void ClientMethod_SetPreferredLanguage_ShouldWorks()
    {
        // Arrange
        Client client = Builder<Client>.CreateNew().Build();
        string lang = "pt-BR";

        // Act
        client.SetPreferredLanguage(lang);

        // Assert
        Assert.AreEqual(lang, client.PreferredLanguage);
    }

    [TestMethod]
    public void ClientMethod_AssociateJuristicClient_ShouldWorks()
    {
        // Arrange
        Client physicalClient = Builder<Client>.CreateNew().Build();
        Client juristicClient = Builder<Client>.CreateNew().Build();

        // Act
        physicalClient.AssociateJuristicClient(juristicClient);

        // Assert
        Assert.AreEqual(juristicClient.Id, physicalClient.JuristicClientId);
        Assert.AreEqual(juristicClient, physicalClient.JuristicClient);
    }

    [TestMethod]
    public void ClientMethod_AssociateLoginUser_ShouldWorks()
    {
        // Arrange
        Client client = Builder<Client>.CreateNew().Build();
        User loginUser = Builder<User>.CreateNew().Build();

        // Act
        client.AssociateLoginUser(loginUser);

        // Assert
        Assert.AreEqual(loginUser.Id, client.LoginUserId);
        Assert.AreEqual(loginUser, client.LoginUser);
    }

    [TestMethod]
    public void ClientMethod_HasFullProfile_ShouldReturnFalse()
    {
        // Arrange
        Client client = Builder<Client>.CreateNew().Build();

        // Act
        bool result = client.HasFullProfile();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ClientMethod_HasFullProfile_ShouldReturnTrue()
    {
        // Arrange
        Client client = new(
            "Ricardo",
            "ricardo@gmail.com",
            "(51) 90000-0001",
            "000.000.000-01",
            new("RS", "Carazinho", "Marcondes", "Edi Marcondes", 33)
        );

        // Act
        bool result = client.HasFullProfile();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ClientMethod_CompleteProfile_ShouldFillDocAndAddres_Successfully()
    {
        // Arrange
        Client client = new(
            "Ricardo",
            "ricardo@gmail.com",
            "(51) 90000-0001"
        );
        string document = "000.000.000-01";
        Address address = new("RS", "Carazinho", "Marcondes", "Edi Marcondes", 33);

        // Act
        client.CompleteProfile(document, address);

        // Assert
        Assert.AreNotEqual(Guid.Empty, client.Id);
        Assert.IsTrue(client.IsActive);
        Assert.AreEqual("Ricardo", client.FullName);
        Assert.AreEqual("ricardo@gmail.com", client.Email);
        Assert.AreEqual("(51) 90000-0001", client.PhoneNumber);
        Assert.IsFalse(client.Type == EClientType.Business);
        Assert.AreEqual("000.000.000-01", client.Document);
        Assert.IsNotNull(client.Address);
        Assert.AreEqual("RS", client.Address.State);
        Assert.AreEqual("Carazinho", client.Address.City);
        Assert.AreEqual("Marcondes", client.Address.Neighborhood);
        Assert.AreEqual("Edi Marcondes", client.Address.Street);
        Assert.AreEqual(33, client.Address.Number);
        Assert.IsNull(client.LicenseNumber);
        Assert.IsNull(client.LicenseValidity);
        Assert.IsNull(client.JuristicClientId);
        Assert.IsNull(client.JuristicClient);
    }

    [TestMethod]
    public void ClientMethod_CompleteProfile_ShouldWorks()
    {
        // Arrange
        Client client = new(
            "Ricardo",
            "ricardo@gmail.com",
            "(51) 90000-0001"
        );
        string document = "000.000.000-01";
        Address address = new("RS", "Carazinho", "Marcondes", "Edi Marcondes", 33);
        string cnh = "123456789";
        DateTimeOffset date = new(2026, 05, 20, 0, 0, 0, TimeSpan.Zero);

        // Act
        client.CompleteProfile(document, address, cnh, date);

        // Assert
        Assert.AreNotEqual(Guid.Empty, client.Id);
        Assert.IsTrue(client.IsActive);
        Assert.AreEqual("Ricardo", client.FullName);
        Assert.AreEqual("ricardo@gmail.com", client.Email);
        Assert.AreEqual("(51) 90000-0001", client.PhoneNumber);
        Assert.IsFalse(client.Type == EClientType.Business);
        Assert.AreEqual("000.000.000-01", client.Document);
        Assert.IsNotNull(client.Address);
        Assert.AreEqual("RS", client.Address.State);
        Assert.AreEqual("Carazinho", client.Address.City);
        Assert.AreEqual("Marcondes", client.Address.Neighborhood);
        Assert.AreEqual("Edi Marcondes", client.Address.Street);
        Assert.AreEqual(33, client.Address.Number);
        Assert.AreEqual(cnh, client.LicenseNumber);
        Assert.AreEqual(date, client.LicenseValidity);
        Assert.IsNull(client.JuristicClientId);
        Assert.IsNull(client.JuristicClient);
    }
    [TestMethod]
    public void ClientMethod_CreateTenantCopyFromGlobal_WithFullProfile_ShouldCopyAllDataAndAssociations()
    {
        // Arrange
        Address globalAddress = new("RS", "Porto Alegre", "Centro", "Av. Borges", 100);
        string doc = "111.222.333-44";
        string cnh = "987654321";
        DateTimeOffset cnhValidity = DateTimeOffset.Now.AddYears(2);

        Client globalClient = new(
            "Cliente Global",
            "global@email.com",
            "(51) 99999-9999",
            doc,
            globalAddress
        );
        globalClient.SetLicense(cnh, cnhValidity);
        globalClient.DefineType(EClientType.Business);

        Guid tenantId = Guid.NewGuid();
        User loginUser = Builder<User>.CreateNew().Build();
        User createdByUser = Builder<User>.CreateNew().Build();

        // Act
        Client tenantClient = Client.CreateTenantCopyFromGlobal(globalClient, tenantId, loginUser, createdByUser);

        // Assert
        Assert.AreNotEqual(globalClient.Id, tenantClient.Id);
        Assert.AreEqual(tenantId, tenantClient.TenantId);
        Assert.AreEqual(globalClient.FullName, tenantClient.FullName);
        Assert.AreEqual(globalClient.Email, tenantClient.Email);
        Assert.AreEqual(globalClient.PhoneNumber, tenantClient.PhoneNumber);
        Assert.AreEqual(EClientType.Business, tenantClient.Type);
        Assert.AreEqual(doc, tenantClient.Document);
        Assert.IsNotNull(tenantClient.Address);
        Assert.AreNotSame(globalClient.Address, tenantClient.Address);
        Assert.AreEqual(globalAddress.State, tenantClient.Address.State);
        Assert.AreEqual(globalAddress.City, tenantClient.Address.City);
        Assert.AreEqual(globalAddress.Street, tenantClient.Address.Street);
        Assert.AreEqual(globalAddress.Number, tenantClient.Address.Number);
        Assert.AreEqual(cnh, tenantClient.LicenseNumber);
        Assert.AreEqual(cnhValidity, tenantClient.LicenseValidity);
        Assert.AreEqual(loginUser.Id, tenantClient.LoginUserId);
        Assert.AreEqual(loginUser, tenantClient.LoginUser);
    }

    [TestMethod]
    public void ClientMethod_CreateTenantCopyFromGlobal_WithPartialProfile_ShouldCopyLicenseAndAssociations()
    {
        // Arrange
        Client globalClient = new(
            "Cliente Parcial",
            "parcial@email.com",
            "(51) 88888-8888"
        );

        string cnh = "123123123";
        DateTimeOffset cnhValidity = DateTimeOffset.Now.AddYears(1);
        globalClient.SetLicense(cnh, cnhValidity);

        Guid tenantId = Guid.NewGuid();
        User loginUser = Builder<User>.CreateNew().Build();
        User createdByUser = Builder<User>.CreateNew().Build();

        // Act
        Client tenantClient = Client.CreateTenantCopyFromGlobal(globalClient, tenantId, loginUser, createdByUser);

        // Assert
        Assert.AreEqual(tenantId, tenantClient.TenantId);
        Assert.AreEqual("Cliente Parcial", tenantClient.FullName);
        Assert.AreEqual("parcial@email.com", tenantClient.Email);
        Assert.IsNull(tenantClient.Document);
        Assert.IsNull(tenantClient.Address);
        Assert.AreEqual(cnh, tenantClient.LicenseNumber);
        Assert.AreEqual(cnhValidity, tenantClient.LicenseValidity);
        Assert.AreEqual(loginUser.Id, tenantClient.LoginUserId);
        Assert.AreEqual(loginUser, tenantClient.LoginUser);
    }
}