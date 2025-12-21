using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.Vehicles;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Vehicles.Application;

[TestClass]
[TestCategory("Vehicle Application - Unit Tests")]
public sealed class GetAllVehicleRequestHandlerTests : UnitTestBase
{
    private GetAllVehicleRequestHandler handler = null!;

    private Mock<IRepositoryVehicle> repositoryVehicleMock = null!;
    private Mock<ILogger<GetAllVehicleRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryVehicleMock = new Mock<IRepositoryVehicle>();
        this.loggerMock = new Mock<ILogger<GetAllVehicleRequestHandler>>();

        this.handler = new GetAllVehicleRequestHandler(
            this.mapper,
            this.repositoryVehicleMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetAllVehicle Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetAllVehicles_Successfully()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        List<Vehicle> vehicles = Builder<Vehicle>.CreateListOfSize(10).All()
            .Do(v => v.SetFuelType(EFuelType.Gasoline))
            .With(v => v.GroupId = groupId).Build().ToList();

        this.repositoryVehicleMock
            .Setup(r => r.GetAllAsync(true))
            .ReturnsAsync(vehicles);

        GetAllVehicleRequest request = new(null, null, null);

        // Act
        Result<GetAllVehicleResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        List<VehicleDto> vehiclesDto = [.. result.Value.Vehicles];

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(10, result.Value.Quantity);
        Assert.AreEqual(vehicles.Count, vehiclesDto.Count);

        for (int i = 0; i < vehicles.Count; i++)
        {
            for (int j = 0; j < vehiclesDto.Count; j++)
            {
                if (vehicles[i].Id == vehiclesDto[j].Id)
                {
                    Assert.AreEqual(vehicles[i].LicensePlate, vehicles[j].LicensePlate);
                    Assert.AreEqual(vehicles[i].Brand, vehicles[j].Brand);
                    Assert.AreEqual(vehicles[i].Color, vehicles[j].Color);
                    Assert.AreEqual(vehicles[i].Model, vehicles[j].Model);
                    Assert.AreEqual(vehicles[i].FuelType, vehicles[j].FuelType);
                    Assert.AreEqual(vehicles[i].FuelTankCapacity, vehicles[j].FuelTankCapacity);
                    Assert.AreEqual(vehicles[i].Year, vehicles[j].Year);
                    Assert.AreEqual(vehicles[i].Image, vehicles[j].Image);
                    Assert.AreEqual(vehicles[i].GroupId, vehicles[j].GroupId);
                }
            }
        }
    }

    [TestMethod]
    public void Handler_ShouldGetFiveVehicles_Successfully()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        List<Vehicle> vehicles = Builder<Vehicle>.CreateListOfSize(10).All()
            .Do(v => v.SetFuelType(EFuelType.Gasoline))
            .With(v => v.GroupId = groupId).Build().ToList();

        this.repositoryVehicleMock
            .Setup(r => r.GetAllAsync(5))
            .ReturnsAsync([.. vehicles.Take(5)]);

        GetAllVehicleRequest request = new(5, null, null);

        // Act
        Result<GetAllVehicleResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        List<VehicleDto> vehiclesDto = [.. result.Value.Vehicles];

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(10, vehicles.Count);
        Assert.AreEqual(5, result.Value.Quantity);
        Assert.AreEqual(5, vehiclesDto.Count);

        for (int i = 0; i < vehicles.Count; i++)
        {
            for (int j = 0; j < vehiclesDto.Count; j++)
            {
                if (vehicles[i].Id == vehiclesDto[j].Id)
                {
                    Assert.AreEqual(vehicles[i].LicensePlate, vehicles[j].LicensePlate);
                    Assert.AreEqual(vehicles[i].Brand, vehicles[j].Brand);
                    Assert.AreEqual(vehicles[i].Color, vehicles[j].Color);
                    Assert.AreEqual(vehicles[i].Model, vehicles[j].Model);
                    Assert.AreEqual(vehicles[i].FuelType, vehicles[j].FuelType);
                    Assert.AreEqual(vehicles[i].FuelTankCapacity, vehicles[j].FuelTankCapacity);
                    Assert.AreEqual(vehicles[i].Year, vehicles[j].Year);
                    Assert.AreEqual(vehicles[i].Image, vehicles[j].Image);
                    Assert.AreEqual(vehicles[i].GroupId, vehicles[j].GroupId);
                }
            }
        }
    }
    #endregion
}
