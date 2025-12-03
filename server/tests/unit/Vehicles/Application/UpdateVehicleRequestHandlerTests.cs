using LocadoraDeAutomoveis.Application.Vehicles.Commands.Update;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;

namespace LocadoraDeAutomoveis.Tests.Unit.Vehicles.Application;

[TestClass]
[TestCategory("Group Application - Unit Tests")]
public sealed class UpdateVehicleRequestHandlerTests
{
    private UpdateVehicleRequestHandler handler = null!;

    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryVehicle> repositoryVehicleMock = null!;
    private Mock<IRepositoryGroup> repositoryGroupMock = null!;
    private Mock<IRepositoryRental> repositoryRentalMock = null!;
    private Mock<IValidator<Vehicle>> validatorMock = null!;
    private Mock<ILogger<UpdateVehicleRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryVehicleMock = new Mock<IRepositoryVehicle>();
        this.repositoryGroupMock = new Mock<IRepositoryGroup>();
        this.repositoryRentalMock = new Mock<IRepositoryRental>();
        this.validatorMock = new Mock<IValidator<Vehicle>>();
        this.loggerMock = new Mock<ILogger<UpdateVehicleRequestHandler>>();

        this.handler = new UpdateVehicleRequestHandler(
            this.unitOfWorkMock.Object,
            this.repositoryVehicleMock.Object,
            this.repositoryGroupMock.Object,
            this.repositoryRentalMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region UpdateVehicle Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldUpdateVehicle_Successfully()
    {
        // Arrange
        Guid vehicleId = Guid.NewGuid();

        Guid groupId = Guid.NewGuid();
        Group group = new("Grupo Ed")
        { Id = groupId };

        UpdateVehicleRequest request = new(
            vehicleId,
            "ABC-1A84",
            "Chevrolet Ed",
            "Preto",
            "Chevette",
            EFuelType.Gasoline,
            45,
            1984,
            null,
            groupId
            );

        Vehicle vehicle = new(
            "CBA-1Z84",
            "Chervrolet",
            "Bege",
            "Chevette",
            45,
            2004,
            string.Empty
        )
        { Id = vehicleId };
        vehicle.SetFuelType(EFuelType.Gas);

        this.repositoryVehicleMock
            .Setup(r => r.GetByIdAsync(vehicleId))
            .ReturnsAsync(vehicle);

        this.repositoryGroupMock
            .Setup(r => r.GetByIdAsync(groupId))
            .ReturnsAsync(group);

        Vehicle updatedVehicle = new(
            request.LicensePlate,
            request.Brand,
            request.Color,
            request.Model,
            request.CapacityInLiters,
            request.Year,
            request.PhotoPath ?? string.Empty
        );
        updatedVehicle.SetFuelType(request.FuelType);

        string expectedPhotoPath = request.PhotoPath ?? string.Empty;
        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<Vehicle>(v =>
                    v.LicensePlate == updatedVehicle.LicensePlate && v.Brand == updatedVehicle.Brand &&
                    v.Color == updatedVehicle.Color && v.Model == updatedVehicle.Model &&
                    v.FuelType == updatedVehicle.FuelType && v.FuelTankCapacity == updatedVehicle.FuelTankCapacity &&
                    v.Year == updatedVehicle.Year && v.PhotoPath == expectedPhotoPath
                    ), CancellationToken.None
                ))
            .ReturnsAsync(new ValidationResult());

        this.repositoryVehicleMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([vehicle]);

        this.repositoryVehicleMock
            .Setup(r => r.UpdateAsync(request.Id, updatedVehicle))
            .Verifiable();

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync());

        // Act
        Result<UpdateVehicleResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.repositoryVehicleMock
            .Verify(r => r.GetByIdAsync(vehicleId),
            Times.Once);

        this.repositoryGroupMock
            .Verify(r => r.GetByIdAsync(groupId),
            Times.Once);

        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.Is<Vehicle>(v =>
                    v.LicensePlate == updatedVehicle.LicensePlate && v.Brand == updatedVehicle.Brand &&
                    v.Color == updatedVehicle.Color && v.Model == updatedVehicle.Model &&
                    v.FuelType == updatedVehicle.FuelType && v.FuelTankCapacity == updatedVehicle.FuelTankCapacity &&
                    v.Year == updatedVehicle.Year && v.PhotoPath == expectedPhotoPath
                    ), CancellationToken.None
                ), Times.Once
            );

        this.repositoryVehicleMock
            .Verify(r => r.GetAllAsync(),
            Times.Once);

        this.repositoryVehicleMock
            .Verify(r => r.UpdateAsync(
                request.Id,
                It.Is<Vehicle>(v =>
                    v.LicensePlate == updatedVehicle.LicensePlate && v.Brand == updatedVehicle.Brand &&
                    v.Color == updatedVehicle.Color && v.Model == updatedVehicle.Model &&
                    v.FuelType == updatedVehicle.FuelType && v.FuelTankCapacity == updatedVehicle.FuelTankCapacity &&
                    v.Year == updatedVehicle.Year && v.PhotoPath == expectedPhotoPath
                    )
                ), Times.Once
            );

        this.unitOfWorkMock.Verify(c => c.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(vehicleId, result.Value.Id);
    }
    #endregion
}
