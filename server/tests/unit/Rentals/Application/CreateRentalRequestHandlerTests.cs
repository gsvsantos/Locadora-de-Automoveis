using LocadoraDeAutomoveis.Application.Rentals.Commands.Create;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.PricingPlans;
using LocadoraDeAutomoveis.Domain.RateServices;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Rentals.Application;

[TestClass]
[TestCategory("Rental Application - Unit Tests")]
public sealed class CreateRentalRequestHandlerTests : UnitTestBase
{
    private CreateRentalRequestHandler handler = null!;

    private const string _userName = "Clebinho";
    private const string _fullName = "Clebinho Da Silva";
    private const string _email = "cleber@dasilva.net";
    private const string _phoneNumber = "(99) 99999-9999";

    private Mock<UserManager<User>> userManagerMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryRental> repositoryRentalMock = null!;
    private Mock<IRepositoryEmployee> repositoryEmployeeMock = null!;
    private Mock<IRepositoryClient> repositoryClientMock = null!;
    private Mock<IRepositoryDriver> repositoryDriverMock = null!;
    private Mock<IRepositoryVehicle> repositoryVehicleMock = null!;
    private Mock<IRepositoryCoupon> repositoryCouponMock = null!;
    private Mock<IRepositoryPricingPlan> repositoryPricingPlanMock = null!;
    private Mock<IRepositoryRateService> repositoryRateServiceMock = null!;
    private Mock<ITenantProvider> tenantProviderMock = null!;
    private Mock<IUserContext> userContextMock = null!;
    private Mock<IValidator<Rental>> validatorMock = null!;
    private Mock<ILogger<CreateRentalRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!
        );

        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryRentalMock = new Mock<IRepositoryRental>();
        this.repositoryEmployeeMock = new Mock<IRepositoryEmployee>();
        this.repositoryClientMock = new Mock<IRepositoryClient>();
        this.repositoryDriverMock = new Mock<IRepositoryDriver>();
        this.repositoryVehicleMock = new Mock<IRepositoryVehicle>();
        this.repositoryCouponMock = new Mock<IRepositoryCoupon>();
        this.repositoryPricingPlanMock = new Mock<IRepositoryPricingPlan>();
        this.repositoryRateServiceMock = new Mock<IRepositoryRateService>();
        this.tenantProviderMock = new Mock<ITenantProvider>();
        this.userContextMock = new Mock<IUserContext>();
        this.validatorMock = new Mock<IValidator<Rental>>();
        this.loggerMock = new Mock<ILogger<CreateRentalRequestHandler>>();

        this.handler = new CreateRentalRequestHandler(
            this.userManagerMock.Object,
            this.unitOfWorkMock.Object,
            this.mapper,
            this.repositoryRentalMock.Object,
            this.repositoryEmployeeMock.Object,
            this.repositoryClientMock.Object,
            this.repositoryDriverMock.Object,
            this.repositoryVehicleMock.Object,
            this.repositoryCouponMock.Object,
            this.repositoryPricingPlanMock.Object,
            this.repositoryRateServiceMock.Object,
            this.tenantProviderMock.Object,
            this.userContextMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region CreateRental Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldCreateRental_WhenAllDataIsValid()
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
        user.AssociateTenant(tenantId);

        this.userContextMock
            .Setup(t => t.GetUserId())
            .Returns(userId);

        this.userManagerMock
            .Setup(u => u.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        Guid clientId = Guid.NewGuid();
        Client client = new(
            "Cliente Teste",
            "email@teste.com",
            "fone",
            "CPF",
            new(
                "RS",
                "Cidade",
                "Bairro",
                "Rua",
                10)
            )
        { Id = clientId };

        this.repositoryClientMock
            .Setup(r => r.GetByIdAsync(clientId))
            .ReturnsAsync(client);

        Guid driverId = Guid.NewGuid();
        Driver driver = new(
            "Motorista Teste",
            "mot@teste.com",
            "fone",
            "CPF",
            "CNH",
            DateTimeOffset.Now
        )
        { Id = driverId };

        this.repositoryDriverMock
            .Setup(r => r.GetByIdAsync(driverId))
            .ReturnsAsync(driver);

        Guid groupId = Guid.NewGuid();
        Group group = new("Grupo Teste") { Id = groupId };

        Guid vehicleId = Guid.NewGuid();
        Vehicle vehicle = new(
            "ABC-1234",
            "Marca",
            "Cor",
            "Modelo",
            50,
            2022,
            "path"
        )
        { Id = vehicleId, GroupId = groupId };

        vehicle.SetFuelType(EFuelType.Gasoline);

        this.repositoryVehicleMock
            .Setup(r => r.GetByIdAsync(vehicleId))
            .ReturnsAsync(vehicle);

        PricingPlan pricingPlan = new(
            "Plano Teste",
            new DailyPlanProps(100, 10),
            new ControlledPlanProps(80, 20),
            new FreePlanProps(200)
        );
        pricingPlan.AssociateGroup(group);

        this.repositoryPricingPlanMock
            .Setup(r => r.GetByGroupId(groupId))
            .ReturnsAsync(pricingPlan);

        List<Guid> serviceIds = [Guid.NewGuid(), Guid.NewGuid()];
        List<RateService> services =
        [
            new RateService("GPS", 10) { IsChargedPerDay = true, RateType = ERateType.Generic },
            new RateService("Cadeira", 20) { IsChargedPerDay = true, RateType = ERateType.Generic }
        ];

        this.repositoryRateServiceMock
            .Setup(r => r.GetManyByIds(serviceIds))
            .ReturnsAsync(services);

        CreateRentalRequest request = new(
            DateTimeOffset.Now,
            DateTimeOffset.Now.AddDays(5),
            1000m,
            null,
            clientId,
            driverId,
            vehicleId,
            null,
            EPricingPlanType.Daily,
            null,
            serviceIds
        );

        this.validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<Rental>(), CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        this.repositoryRentalMock
            .Setup(r => r.AddAsync(It.IsAny<Rental>()))
            .Verifiable();

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync())
            .Verifiable();

        // Act
        Result<CreateRentalResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.userContextMock
            .Verify(u => u.GetUserId(), Times.Once);

        this.userManagerMock
            .Verify(r => r.FindByIdAsync(userId.ToString()), Times.Once);

        this.repositoryClientMock
            .Verify(r => r.GetByIdAsync(clientId), Times.Once);

        this.repositoryDriverMock
            .Verify(r => r.GetByIdAsync(driverId), Times.Once);

        this.repositoryVehicleMock
            .Verify(r => r.GetByIdAsync(vehicleId), Times.Once);

        this.repositoryPricingPlanMock
            .Verify(r => r.GetByGroupId(groupId), Times.Once);

        this.repositoryRateServiceMock
            .Verify(r => r.GetManyByIds(serviceIds), Times.Once);

        this.validatorMock
            .Verify(v => v.ValidateAsync(
                    It.IsAny<Rental>(), CancellationToken.None
                ),
                Times.Once
            );

        this.repositoryRentalMock
            .Verify(r => r.AddAsync(
                    It.Is<Rental>(rental =>
                        rental.ClientId == clientId &&
                        rental.DriverId == driverId &&
                        rental.VehicleId == vehicleId &&
                        rental.RateServices.Count == 2 &&
                        rental.Status == ERentalStatus.Open &&
                        rental.SelectedPlanType == EPricingPlanType.Daily
                    )
                ),
                Times.Once
            );

        this.unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
