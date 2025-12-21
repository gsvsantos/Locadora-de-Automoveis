using LocadoraDeAutomoveis.Application.Vehicles.Commands.Update;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using LocadoraDeAutomoveis.Infrastructure.S3;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Vehicles.Application;

[TestClass]
[TestCategory("Group Application - Unit Tests")]
public sealed class UpdateVehicleRequestHandlerTests : UnitTestBase
{
    private UpdateVehicleRequestHandler handler = null!;

    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryVehicle> repositoryVehicleMock = null!;
    private Mock<IRepositoryGroup> repositoryGroupMock = null!;
    private Mock<IRepositoryRental> repositoryRentalMock = null!;
    private Mock<IR2FileStorageService> fileStorageMock = null!;
    private Mock<ITenantProvider> tenantProviderMock = null!;
    private Mock<IValidator<Vehicle>> validatorMock = null!;
    private Mock<ILogger<UpdateVehicleRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryVehicleMock = new Mock<IRepositoryVehicle>();
        this.repositoryGroupMock = new Mock<IRepositoryGroup>();
        this.repositoryRentalMock = new Mock<IRepositoryRental>();
        this.fileStorageMock = new Mock<IR2FileStorageService>();
        this.tenantProviderMock = new Mock<ITenantProvider>();
        this.validatorMock = new Mock<IValidator<Vehicle>>();
        this.loggerMock = new Mock<ILogger<UpdateVehicleRequestHandler>>();

        this.handler = new UpdateVehicleRequestHandler(
            this.unitOfWorkMock.Object,
            this.mapper,
            this.repositoryVehicleMock.Object,
            this.repositoryGroupMock.Object,
            this.repositoryRentalMock.Object,
            this.fileStorageMock.Object,
            this.tenantProviderMock.Object,
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
            500,
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
            1000,
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

        string expectedPhotoPath = vehicle.Image ?? string.Empty;
        Vehicle updatedVehicle = new(
            request.LicensePlate,
            request.Brand,
            request.Color,
            request.Model,
            request.FuelTankCapacity,
            request.Kilometers,
            request.Year,
            expectedPhotoPath
        );

        updatedVehicle.SetFuelType(request.FuelType);
        this.validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
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

        this.validatorMock.Verify(
            v => v.ValidateAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()),
            Times.Once
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
                    v.Year == updatedVehicle.Year && v.Image == expectedPhotoPath
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
