using LocadoraDeAutomoveis.Application.Clients.Commands.Delete;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Clients.Application;

[TestClass]
[TestCategory("Client Application - Unit Tests")]
public sealed class DeleteClientRequestHandlerTests
{
    private DeleteClientRequestHandler handler = null!;

    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryClient> repositoryClientMock = null!;
    private Mock<IRepositoryRental> repositoryRentalMock = null!;
    private Mock<ILogger<DeleteClientRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryClientMock = new Mock<IRepositoryClient>();
        this.repositoryRentalMock = new Mock<IRepositoryRental>();
        this.loggerMock = new Mock<ILogger<DeleteClientRequestHandler>>();

        this.handler = new DeleteClientRequestHandler(
            this.unitOfWorkMock.Object,
            this.repositoryClientMock.Object,
            this.repositoryRentalMock.Object,
            this.loggerMock.Object
        );
    }

    #region DeleteClient Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldDeleteClient_Successfully()
    {
        // Arrange
        Guid clientId = Guid.NewGuid();
        DeleteClientRequest request = new(clientId);

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

        this.repositoryClientMock
            .Setup(r => r.DeleteAsync(request.Id))
            .Verifiable();

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync())
            .Verifiable();

        // Act
        Result<DeleteClientResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.repositoryClientMock
            .Verify(r => r.GetByIdAsync(clientId),
            Times.Once);

        this.repositoryClientMock
            .Verify(r => r.DeleteAsync(clientId),
            Times.Once);

        this.unitOfWorkMock.Verify(c => c.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
