using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetById;
using LocadoraDeAutomoveis.Domain.Vehicles;

namespace LocadoraDeAutomoveis.Tests.Unit.Vehicles.Application;

[TestClass]
[TestCategory("Vehicle Application - Unit Tests")]
public sealed class GetByIdVehicleRequestHandlerTests
{
    private GetByIdVehicleRequestHandler handler = null!;

    private Mock<IRepositoryVehicle> repositoryVehicleMock = null!;
    private Mock<ILogger<GetByIdVehicleRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryVehicleMock = new Mock<IRepositoryVehicle>();
        this.loggerMock = new Mock<ILogger<GetByIdVehicleRequestHandler>>();

        this.handler = new GetByIdVehicleRequestHandler(
            this.repositoryVehicleMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetVehicleById Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetVehicleById_Successfuly()
    {
        // Arrange
        Guid vehicleId = Guid.NewGuid();
        GetByIdVehicleRequest request = new(vehicleId);

        Vehicle vehicle = new(
            "ABC-1A84",
            "Chevrolet Ed",
            "Preto",
            "Chevette",
            "Gasolina",
            45,
            1984,
            string.Empty
        )
        { Id = vehicleId };

        this.repositoryVehicleMock
            .Setup(rr => rr.GetByIdAsync(request.Id))
            .ReturnsAsync(vehicle);

        // Act
        Result<GetByIdVehicleResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        VehicleDto dto = result.Value.Vehicle;

        // Assert
        this.repositoryVehicleMock
            .Verify(r => r.GetByIdAsync(request.Id), Times.Once);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(vehicle.Id, dto.Id);
        Assert.AreEqual(vehicle.LicensePlate, dto.LicensePlate);
        Assert.AreEqual(vehicle.Brand, dto.Brand);
        Assert.AreEqual(vehicle.Color, dto.Color);
        Assert.AreEqual(vehicle.Model, dto.Model);
        Assert.AreEqual(vehicle.FuelType, dto.FuelType);
        Assert.AreEqual(vehicle.CapacityInLiters, dto.CapacityInLiters);
        Assert.AreEqual(vehicle.Year, dto.Year);
        Assert.AreEqual(vehicle.PhotoPath, dto.PhotoPath);
    }
    #endregion
}
