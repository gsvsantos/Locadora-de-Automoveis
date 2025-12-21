using LocadoraDeAutomoveis.Application.Vehicles.Commands.Create;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using LocadoraDeAutomoveis.Infrastructure.S3;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Vehicles.Application;

[TestClass]
[TestCategory("Vehicle Application - Unit Tests")]
public sealed class CreateVehicleRequestHandlerTests : UnitTestBase
{
    private CreateVehicleRequestHandler handler = null!;

    private const string _userName = "Clebinho";
    private const string _fullName = "Clebinho Da Silva";
    private const string _email = "cleber@dasilva.net";
    private const string _phoneNumber = "(99) 99999-9999";

    private Mock<UserManager<User>> userManagerMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryVehicle> repositoryVehicleMock = null!;
    private Mock<IRepositoryGroup> repositoryGroupMock = null!;
    private Mock<IR2FileStorageService> fileStorageMock = null!;
    private Mock<ITenantProvider> tenantProviderMock = null!;
    private Mock<IUserContext> userContextMock = null!;
    private Mock<IValidator<Vehicle>> validatorMock = null!;
    private Mock<ILogger<CreateVehicleRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!
        );

        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryVehicleMock = new Mock<IRepositoryVehicle>();
        this.repositoryGroupMock = new Mock<IRepositoryGroup>();
        this.fileStorageMock = new Mock<IR2FileStorageService>();
        this.tenantProviderMock = new Mock<ITenantProvider>();
        this.userContextMock = new Mock<IUserContext>();
        this.validatorMock = new Mock<IValidator<Vehicle>>();
        this.loggerMock = new Mock<ILogger<CreateVehicleRequestHandler>>();

        this.handler = new CreateVehicleRequestHandler(
            this.userManagerMock.Object,
            this.unitOfWorkMock.Object,
            this.mapper,
            this.repositoryVehicleMock.Object,
            this.repositoryGroupMock.Object,
            this.fileStorageMock.Object,
            this.tenantProviderMock.Object,
            this.userContextMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region CreateVehicle Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldCreateVehicle_Successfully()
    {
        // Arrange
        Guid tenantId = Guid.NewGuid();
        this.tenantProviderMock
            .Setup(t => t.GetTenantId())
            .Returns(tenantId);

        Guid userId = Guid.NewGuid();
        User user = new()
        {
            Id = userId,
            UserName = _userName,
            FullName = _fullName,
            Email = _email,
            PhoneNumber = _phoneNumber,
        };

        Guid groupId = Guid.NewGuid();
        Group group = new("Group A") { Id = groupId };
        group.AssociateTenant(tenantId);
        group.AssociateUser(user);

        this.repositoryGroupMock
            .Setup(r => r.GetByIdAsync(groupId))
            .ReturnsAsync(group);

        CreateVehicleRequest request = new(
            "ABC-1A84",
            "Chevrolet",
            "Bege",
            "Chevette",
            EFuelType.Gasoline,
            45,
            1000,
            1984,
            null,
            groupId
        );

        this.userContextMock
            .Setup(t => t.GetUserId())
            .Returns(userId);

        this.userManagerMock
            .Setup(u => u.FindByIdAsync(this.userContextMock.Object.GetUserId().ToString()))
            .ReturnsAsync(user);

        string expectedPhotoPath = "fake-image-key.jpg";
        Vehicle vehicle = new(
            request.LicensePlate,
            request.Brand,
            request.Color,
            request.Model,
            request.FuelTankCapacity,
            request.Kilometers,
            request.Year,
            expectedPhotoPath
        );
        vehicle.AssociateTenant(tenantId);
        vehicle.AssociateUser(user);
        vehicle.AssociateGroup(group);

        this.validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        this.repositoryVehicleMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        this.repositoryVehicleMock
            .Setup(r => r.AddAsync(It.Is<Vehicle>(createdVehicle =>
                createdVehicle.LicensePlate == request.LicensePlate &&
                createdVehicle.Brand == request.Brand &&
                createdVehicle.Color == request.Color &&
                createdVehicle.Model == request.Model &&
                createdVehicle.FuelType == request.FuelType &&
                createdVehicle.FuelTankCapacity == request.FuelTankCapacity &&
                createdVehicle.Year == request.Year &&
                string.IsNullOrEmpty(createdVehicle.Image) &&
                createdVehicle.GroupId == groupId
            )))
            .Verifiable();

        // Act
        Result<CreateVehicleResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.userManagerMock
            .Verify(r => r.FindByIdAsync(this.userContextMock.Object.GetUserId().ToString()),
            Times.Once);

        this.repositoryGroupMock
            .Verify(r => r.GetByIdAsync(groupId),
            Times.Once);

        this.fileStorageMock.Verify(
            s => s.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        this.validatorMock.Verify(
            v => v.ValidateAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()),
            Times.Once
        );

        this.repositoryVehicleMock
            .Verify(r => r.GetAllAsync(), Times.Once);

        this.repositoryVehicleMock
            .Verify(r => r.AddAsync(
                It.Is<Vehicle>(createdVehicle =>
                        createdVehicle.LicensePlate == request.LicensePlate &&
                        createdVehicle.Brand == request.Brand &&
                        createdVehicle.Color == request.Color &&
                        createdVehicle.Model == request.Model &&
                        createdVehicle.FuelType == request.FuelType &&
                        createdVehicle.FuelTankCapacity == request.FuelTankCapacity &&
                        createdVehicle.Year == request.Year &&
                        string.IsNullOrEmpty(createdVehicle.Image) &&
                        createdVehicle.GroupId == groupId
                    )
                ), Times.Once
            );

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
