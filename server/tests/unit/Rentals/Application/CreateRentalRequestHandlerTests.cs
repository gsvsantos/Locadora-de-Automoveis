using LocadoraDeAutomoveis.Application.Rentals.Commands.Create;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.RentalExtras;
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
    private Mock<IRepositoryBillingPlan> repositoryBillingPlanMock = null!;
    private Mock<IRepositoryRentalExtra> repositoryRentalExtraMock = null!;
    private Mock<ITenantProvider> tenantProviderMock = null!;
    private Mock<IUserContext> userContextMock = null!;
    private Mock<IValidator<Rental>> validatorMock = null!;
    private Mock<IRentalEmailService> emailServiceMock = null!;
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
        this.repositoryBillingPlanMock = new Mock<IRepositoryBillingPlan>();
        this.repositoryRentalExtraMock = new Mock<IRepositoryRentalExtra>();
        this.tenantProviderMock = new Mock<ITenantProvider>();
        this.userContextMock = new Mock<IUserContext>();
        this.validatorMock = new Mock<IValidator<Rental>>();

        this.emailServiceMock = new Mock<IRentalEmailService>();
        this.emailServiceMock
            .Setup(s => s.ScheduleRentalConfirmation(It.IsAny<Rental>(), It.IsAny<Client>()))
            .Returns(Task.CompletedTask);

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
            this.repositoryBillingPlanMock.Object,
            this.repositoryRentalExtraMock.Object,
            this.tenantProviderMock.Object,
            this.userContextMock.Object,
            this.emailServiceMock.Object,
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

        BillingPlan BillingPlan = new(
            "Plano Teste",
            new DailyBilling(100, 10),
            new ControlledBilling(80, 20),
            new FreeBilling(200)
        );
        BillingPlan.AssociateGroup(group);

        this.repositoryBillingPlanMock
            .Setup(r => r.GetByGroupId(groupId))
            .ReturnsAsync(BillingPlan);

        List<Guid> extrasIds = [Guid.NewGuid(), Guid.NewGuid()];
        List<RentalExtra> extras =
        [
            new RentalExtra("GPS", 10) { IsDaily = true, Type = EExtraType.Service },
            new RentalExtra("Cadeira", 20) { IsDaily = true, Type = EExtraType.Equipment }
        ];

        this.repositoryRentalExtraMock
            .Setup(r => r.GetManyByIds(extrasIds))
            .ReturnsAsync(extras);

        CreateRentalRequest request = new(
            DateTimeOffset.Now,
            DateTimeOffset.Now.AddDays(5),
            1000m,
            null,
            clientId,
            driverId,
            vehicleId,
            null,
            EBillingPlanType.Daily,
            null,
            extrasIds
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

        this.repositoryBillingPlanMock
            .Verify(r => r.GetByGroupId(groupId), Times.Once);

        this.repositoryRentalExtraMock
            .Verify(r => r.GetManyByIds(extrasIds), Times.Once);

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
                        rental.Extras.Count == 2 &&
                        rental.Status == ERentalStatus.Open &&
                        rental.BillingPlanType == EBillingPlanType.Daily
                    )
                ),
                Times.Once
            );

        this.unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
