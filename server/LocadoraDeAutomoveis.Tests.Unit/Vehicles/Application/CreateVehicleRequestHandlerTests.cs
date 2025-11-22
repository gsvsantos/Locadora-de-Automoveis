using LocadoraDeAutomoveis.Application.Vehicles.Commands.Create;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;

namespace LocadoraDeAutomoveis.Tests.Unit.Vehicles.Application;

[TestClass]
[TestCategory("Vehicle Application - Unit Tests")]
public sealed class CreateVehicleRequestHandlerTests
{
    private CreateVehicleRequestHandler handler = null!;

    private const string _userName = "Clebinho";
    private const string _fullName = "Clebinho Da Silva";
    private const string _email = "cleber@dasilva.net";
    private const string _phoneNumber = "(99) 99999-9999";

    private Mock<UserManager<User>> userManagerMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryVehicle> repositoryVehicleMock = null!;
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
        this.tenantProviderMock = new Mock<ITenantProvider>();
        this.userContextMock = new Mock<IUserContext>();
        this.validatorMock = new Mock<IValidator<Vehicle>>();
        this.loggerMock = new Mock<ILogger<CreateVehicleRequestHandler>>();

        this.handler = new CreateVehicleRequestHandler(
            this.userManagerMock.Object,
            this.unitOfWorkMock.Object,
            this.repositoryVehicleMock.Object,
            this.tenantProviderMock.Object,
            this.userContextMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region CreateGroup Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldCreateGroup_Successfully()
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

        DateTimeOffset date = new(1984, 1, 1, 0, 0, 0, TimeSpan.Zero);
        CreateVehicleRequest request = new(
            "ABC-1A84",
            "Chevrolet",
            "Bege",
            "Chevette",
            "Gasolina",
            45,
            date,
            null,
            groupId
        );

        this.userContextMock
            .Setup(t => t.GetUserId())
            .Returns(userId);

        this.userManagerMock
            .Setup(u => u.FindByIdAsync(this.userContextMock.Object.GetUserId().ToString()))
            .ReturnsAsync(user);

        string expectedPhotoPath = request.PhotoPath ?? string.Empty;
        Vehicle vehicle = new(
            request.LicensePlate,
            request.Brand,
            request.Color,
            request.Model,
            request.FuelType,
            request.CapacityInLiters,
            request.Year,
            request.PhotoPath ?? string.Empty
        );
        vehicle.AssociateTenant(tenantId);
        vehicle.AssociateUser(user);
        vehicle.AssociateGroup(group);

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<Vehicle>(v =>
                    v.LicensePlate == request.LicensePlate && v.Brand == request.Brand &&
                    v.Color == request.Color && v.Model == request.Model &&
                    v.FuelType == request.FuelType && v.CapacityInLiters == request.CapacityInLiters &&
                    v.Year == request.Year && v.PhotoPath == expectedPhotoPath
                    ), CancellationToken.None
                ))
            .ReturnsAsync(new ValidationResult());

        this.repositoryVehicleMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        this.repositoryVehicleMock
            .Setup(r => r.AddAsync(
                It.Is<Vehicle>(v =>
                    v.LicensePlate == request.LicensePlate && v.Brand == request.Brand &&
                    v.Color == request.Color && v.Model == request.Model &&
                    v.FuelType == request.FuelType && v.CapacityInLiters == request.CapacityInLiters &&
                    v.Year == request.Year && v.PhotoPath == expectedPhotoPath
                    ))
            );

        // Act
        Result<CreateVehicleResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.Is<Vehicle>(v =>
                    v.LicensePlate == request.LicensePlate && v.Brand == request.Brand &&
                    v.Color == request.Color && v.Model == request.Model &&
                    v.FuelType == request.FuelType && v.CapacityInLiters == request.CapacityInLiters &&
                    v.Year == request.Year && v.PhotoPath == expectedPhotoPath
                    ), CancellationToken.None
                ), Times.Once
            );

        this.repositoryVehicleMock
            .Verify(r => r.GetAllAsync(), Times.Once);

        this.repositoryVehicleMock
            .Verify(r => r.AddAsync(
                It.Is<Vehicle>(v =>
                    v.LicensePlate == request.LicensePlate && v.Brand == request.Brand &&
                    v.Color == request.Color && v.Model == request.Model &&
                    v.FuelType == request.FuelType && v.CapacityInLiters == request.CapacityInLiters &&
                    v.Year == request.Year && v.PhotoPath == expectedPhotoPath
                    )), Times.Once
            );

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
