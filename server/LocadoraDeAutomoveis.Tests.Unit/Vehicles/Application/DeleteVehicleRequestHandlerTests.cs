using LocadoraDeAutomoveis.Application.Vehicles.Commands.Delete;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;

namespace LocadoraDeAutomoveis.Tests.Unit.Vehicles.Application;

[TestClass]
[TestCategory("Vehicle Application - Unit Tests")]
public sealed class DeleteVehicleRequestHandlerTests
{
    private DeleteVehicleRequestHandler handler = null!;

    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryVehicle> repositoryVehicleMock = null!;
    private Mock<ILogger<DeleteVehicleRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryVehicleMock = new Mock<IRepositoryVehicle>();
        this.loggerMock = new Mock<ILogger<DeleteVehicleRequestHandler>>();

        this.handler = new DeleteVehicleRequestHandler(
            this.unitOfWorkMock.Object,
            this.repositoryVehicleMock.Object,
            this.loggerMock.Object
        );
    }

    #region DeleteVehicle Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldDeleteVehicle_Successfully()
    {
        // Arrange
        Guid vehicleId = Guid.NewGuid();
        DeleteVehicleRequest request = new(vehicleId);

        Vehicle vehicle = new(
            "ABC-1A84",
            "Chevrolet Ed",
            "Preto",
            "Chevette",
            "Gasolina",
            45,
            new(1984, 1, 1, 0, 0, 0, TimeSpan.Zero),
            string.Empty
        )
        { Id = vehicleId };

        this.repositoryVehicleMock
            .Setup(r => r.GetByIdAsync(vehicleId))
            .ReturnsAsync(vehicle);

        this.repositoryVehicleMock
            .Setup(r => r.DeleteAsync(request.Id))
            .Verifiable();

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync())
            .Verifiable();

        // Act
        Result<DeleteVehicleResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.repositoryVehicleMock
            .Verify(r => r.GetByIdAsync(vehicleId),
            Times.Once);

        this.repositoryVehicleMock
            .Verify(r => r.DeleteAsync(request.Id),
            Times.Once);

        this.unitOfWorkMock.Verify(c => c.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
