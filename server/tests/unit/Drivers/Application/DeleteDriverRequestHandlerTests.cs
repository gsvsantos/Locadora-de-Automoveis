using LocadoraDeAutomoveis.Application.Drivers.Commands.Delete;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Drivers.Application;

[TestClass]
[TestCategory("Driver Application - Unit Tests")]
public sealed class DeleteDriverRequestHandlerTests
{
    private DeleteDriverRequestHandler handler = null!;

    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryDriver> repositoryDriverMock = null!;
    private Mock<IRepositoryRental> repositoryRentalMock = null!;
    private Mock<ILogger<DeleteDriverRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryDriverMock = new Mock<IRepositoryDriver>();
        this.repositoryRentalMock = new Mock<IRepositoryRental>();
        this.loggerMock = new Mock<ILogger<DeleteDriverRequestHandler>>();

        this.handler = new DeleteDriverRequestHandler(
            this.unitOfWorkMock.Object,
            this.repositoryDriverMock.Object,
            this.repositoryRentalMock.Object,
            this.loggerMock.Object
        );
    }

    #region DeleteDriver Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldDeleteDriver_Successfully()
    {
        // Arrange
        Guid driverId = Guid.NewGuid();
        DeleteDriverRequest request = new(driverId);

        Driver driver = new(
            "Cliente Novo",
            "clienteNovo@email.com",
            "(51) 99999-9999",
            "222.222.222-22",
            "12345",
            DateTimeOffset.Now
        )
        { Id = driverId };

        this.repositoryDriverMock
            .Setup(r => r.GetByIdAsync(driverId))
            .ReturnsAsync(driver);

        this.repositoryDriverMock
            .Setup(r => r.DeleteAsync(request.Id))
            .Verifiable();

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync())
            .Verifiable();

        // Act
        Result<DeleteDriverResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.repositoryDriverMock
            .Verify(r => r.GetByIdAsync(driverId),
            Times.Once);

        this.repositoryDriverMock
            .Verify(r => r.DeleteAsync(request.Id),
            Times.Once);

        this.unitOfWorkMock.Verify(c => c.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
