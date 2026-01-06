using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Application.Clients.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Tests.Unit.Shared;
using Microsoft.Extensions.Caching.Distributed;

namespace LocadoraDeAutomoveis.Tests.Unit.Clients.Application;

[TestClass]
[TestCategory("Client Application - Unit Tests")]
public sealed class GetAllClientRequestHandlerTests : UnitTestBase
{
    private GetAllClientRequestHandler handler = null!;

    private Mock<IRepositoryClient> repositoryClientMock = null!;
    private Mock<IDistributedCache> cacheMock = null!;
    private Mock<ILogger<GetAllClientRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryClientMock = new Mock<IRepositoryClient>();
        this.cacheMock = new Mock<IDistributedCache>();
        this.loggerMock = new Mock<ILogger<GetAllClientRequestHandler>>();

        this.handler = new GetAllClientRequestHandler(
            this.mapper,
            this.repositoryClientMock.Object,
            this.cacheMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetClientPlans Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetClients_Successfully()
    {
        // Arrange
        RandomGenerator random = new();
        List<Client> clients = Builder<Client>.CreateListOfSize(10).All()
            .Do(c => c.Address = new Address(
                random.NextString(5, 5),
                random.NextString(5, 5),
                random.NextString(5, 5),
                random.NextString(5, 5),
                random.Int()
                ))
            .Build().ToList();

        this.repositoryClientMock
            .Setup(r => r.GetAllAsync(true))
            .ReturnsAsync(clients);

        GetAllClientRequest request = new(null, null);

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
                    Assert.AreEqual(clients[i].Address!.State, clients[j].Address!.State);
                    Assert.AreEqual(clients[i].Address!.City, clients[j].Address!.City);
                    Assert.AreEqual(clients[i].Address!.Neighborhood, clients[j].Address!.Neighborhood);
                    Assert.AreEqual(clients[i].Address!.Street, clients[j].Address!.Street);
                    Assert.AreEqual(clients[i].Address!.Number, clients[j].Address!.Number);
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
            .Do(c => c.Address = new Address(
                random.NextString(5, 5),
                random.NextString(5, 5),
                random.NextString(5, 5),
                random.NextString(5, 5),
                random.Int()
                ))
            .Build().ToList();

        this.repositoryClientMock
            .Setup(r => r.GetAllAsync(5))
            .ReturnsAsync([.. clients.Take(5)]);

        GetAllClientRequest request = new(5, null);

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
                    Assert.AreEqual(clients[i].Address!.State, clients[j].Address!.State);
                    Assert.AreEqual(clients[i].Address!.City, clients[j].Address!.City);
                    Assert.AreEqual(clients[i].Address!.Neighborhood, clients[j].Address!.Neighborhood);
                    Assert.AreEqual(clients[i].Address!.Street, clients[j].Address!.Street);
                    Assert.AreEqual(clients[i].Address!.Number, clients[j].Address!.Number);
                    Assert.AreEqual(clients[i].Document, clients[j].Document);
                }
            }
        }
    }
    #endregion
}
