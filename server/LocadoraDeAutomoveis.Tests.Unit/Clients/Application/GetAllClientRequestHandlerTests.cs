using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Application.Clients.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.Clients;

namespace LocadoraDeAutomoveis.Tests.Unit.Clients.Application;

[TestClass]
[TestCategory("Client Application - Unit Tests")]
public sealed class GetAllClientRequestHandlerTests
{
    private GetAllClientRequestHandler handler = null!;

    private Mock<IRepositoryClient> repositoryClientMock = null!;
    private Mock<ILogger<GetAllClientRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryClientMock = new Mock<IRepositoryClient>();
        this.loggerMock = new Mock<ILogger<GetAllClientRequestHandler>>();

        this.handler = new GetAllClientRequestHandler(
            this.repositoryClientMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetClientPlans Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetClients_Successfully()
    {
        // Arrange
        List<Client> clients = Builder<Client>.CreateListOfSize(10).All()
            .Build().ToList();

        this.repositoryClientMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(clients);

        GetAllClientRequest request = new(null);

        // Act
        Result<GetAllClientResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        List<ClientDto> clientsDto = [.. result.Value.Clients];

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(10, result.Value.Quantity);
        Assert.AreEqual(clients.Count, clientsDto.Count);

        for (int i = 0; i < clients.Count; i++)
        {
            for (int j = 0; j < clientsDto.Count; j++)
            {
                if (clients[i].Id == clientsDto[j].Id)
                {
                    Assert.AreEqual(clients[i].FullName, clients[j].FullName);
                    Assert.AreEqual(clients[i].Email, clients[j].Email);
                    Assert.AreEqual(clients[i].PhoneNumber, clients[j].PhoneNumber);
                    Assert.AreEqual(clients[i].State, clients[j].State);
                    Assert.AreEqual(clients[i].City, clients[j].City);
                    Assert.AreEqual(clients[i].Neighborhood, clients[j].Neighborhood);
                    Assert.AreEqual(clients[i].Street, clients[j].Street);
                    Assert.AreEqual(clients[i].Number, clients[j].Number);
                    Assert.AreEqual(clients[i].Document, clients[j].Document);
                }
            }
        }
    }

    [TestMethod]
    public void Handler_ShouldGetFiveClients_Successfully()
    {
        // Arrange
        RandomGenerator random = new();
        List<Client> clients = Builder<Client>.CreateListOfSize(10).All()
            .Build().ToList();

        this.repositoryClientMock
            .Setup(r => r.GetAllAsync(5))
            .ReturnsAsync([.. clients.Take(5)]);

        GetAllClientRequest request = new(5);

        // Act
        Result<GetAllClientResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        List<ClientDto> clientsDto = [.. result.Value.Clients];

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(10, clients.Count);
        Assert.AreEqual(5, result.Value.Quantity);
        Assert.AreEqual(5, clientsDto.Count);

        for (int i = 0; i < clients.Count; i++)
        {
            for (int j = 0; j < clientsDto.Count; j++)
            {
                if (clients[i].Id == clientsDto[j].Id)
                {
                    Assert.AreEqual(clients[i].FullName, clients[j].FullName);
                    Assert.AreEqual(clients[i].Email, clients[j].Email);
                    Assert.AreEqual(clients[i].PhoneNumber, clients[j].PhoneNumber);
                    Assert.AreEqual(clients[i].State, clients[j].State);
                    Assert.AreEqual(clients[i].City, clients[j].City);
                    Assert.AreEqual(clients[i].Neighborhood, clients[j].Neighborhood);
                    Assert.AreEqual(clients[i].Street, clients[j].Street);
                    Assert.AreEqual(clients[i].Number, clients[j].Number);
                    Assert.AreEqual(clients[i].Document, clients[j].Document);
                }
            }
        }
    }
    #endregion
}
