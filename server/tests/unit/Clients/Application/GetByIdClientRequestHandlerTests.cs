using LocadoraDeAutomoveis.Application.Clients.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Clients.Commands.GetById;
using LocadoraDeAutomoveis.Domain.Clients;

namespace LocadoraDeAutomoveis.Tests.Unit.Clients.Application;

[TestClass]
[TestCategory("Client Application - Unit Tests")]
public sealed class GetByIdClientRequestHandlerTests
{
    private GetByIdClientRequestHandler handler = null!;

    private Mock<IRepositoryClient> repositoryClientMock = null!;
    private Mock<ILogger<GetByIdClientRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryClientMock = new Mock<IRepositoryClient>();
        this.loggerMock = new Mock<ILogger<GetByIdClientRequestHandler>>();

        this.handler = new GetByIdClientRequestHandler(
            this.repositoryClientMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetClientById Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetClientById_Successfuly()
    {
        // Arrange
        Guid clientId = Guid.NewGuid();
        GetByIdClientRequest request = new(clientId);
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
        )
        { Id = clientId };

        this.repositoryClientMock
            .Setup(r => r.GetByIdAsync(clientId))
            .ReturnsAsync(client);

        // Act
        Result<GetByIdClientResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        ClientDto dto = result.Value.Client;

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(client.Id, dto.Id);
        Assert.AreEqual(client.FullName, dto.FullName);
        Assert.AreEqual(client.Email, dto.Email);
        Assert.AreEqual(client.PhoneNumber, dto.PhoneNumber);
        Assert.AreEqual(client.Address.State, dto.Address.State);
        Assert.AreEqual(client.Address.City, dto.Address.City);
        Assert.AreEqual(client.Address.Neighborhood, dto.Address.Neighborhood);
        Assert.AreEqual(client.Address.Street, dto.Address.Street);
        Assert.AreEqual(client.Address.Number, dto.Address.Number);
        Assert.AreEqual(client.Document, dto.Document);
    }
    #endregion
}
