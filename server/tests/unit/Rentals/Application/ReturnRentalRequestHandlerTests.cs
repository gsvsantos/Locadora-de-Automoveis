using LocadoraDeAutomoveis.Application.Rentals.Commands.Return;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Configurations;
using LocadoraDeAutomoveis.Domain.PricingPlans;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Rentals.Application;

[TestClass]
[TestCategory("Rental Application - Unit Tests")]
public sealed class ReturnRentalRequestHandlerTests : UnitTestBase
{
    private ReturnRentalRequestHandler handler = null!;

    private Mock<UserManager<User>> userManagerMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryRentalReturn> repositoryRentalReturnMock = null!;
    private Mock<IRepositoryRental> repositoryRentalMock = null!;
    private Mock<IRepositoryConfiguration> repositoryConfigurationMock = null!;
    private Mock<ITenantProvider> tenantProviderMock = null!;
    private Mock<IUserContext> userContextMock = null!;
    private Mock<IValidator<RentalReturn>> validatorMock = null!;
    private Mock<ILogger<ReturnRentalRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!
        );

        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryRentalReturnMock = new Mock<IRepositoryRentalReturn>();
        this.repositoryRentalMock = new Mock<IRepositoryRental>();
        this.repositoryConfigurationMock = new Mock<IRepositoryConfiguration>();
        this.tenantProviderMock = new Mock<ITenantProvider>();
        this.userContextMock = new Mock<IUserContext>();
        this.validatorMock = new Mock<IValidator<RentalReturn>>();
        this.loggerMock = new Mock<ILogger<ReturnRentalRequestHandler>>();

        this.handler = new ReturnRentalRequestHandler(
            this.userManagerMock.Object,
            this.unitOfWorkMock.Object,
            this.mapper,
            this.repositoryRentalReturnMock.Object,
            this.repositoryRentalMock.Object,
            this.repositoryConfigurationMock.Object,
            this.tenantProviderMock.Object,
            this.userContextMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    [TestMethod]
    public void Handler_ShouldReturnRental_Successfully()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid tenantId = Guid.NewGuid();
        Guid rentalId = Guid.NewGuid();

        this.userContextMock
            .Setup(u => u.GetUserId())
            .Returns(userId);

        this.tenantProviderMock
            .Setup(t => t.GetTenantId())
            .Returns(tenantId);

        User user = new() { Id = userId };
        this.userManagerMock
            .Setup(u => u.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        Configuration config = new()
        {
            GasolinePrice = 5.50m,
            DieselPrice = 4.50m,
            AlcoholPrice = 3.80m,
            GasPrice = 3.00m
        };
        this.repositoryConfigurationMock
            .Setup(r => r.GetByTenantId(tenantId))
            .ReturnsAsync(config);

        PricingPlan pricingPlan = new(
            "Plano Teste",
            new DailyPlanProps(100m, 10m),
            new ControlledPlanProps(80m, 20m),
            new FreePlanProps(200m)
        );

        Vehicle vehicle = new(
            "ABC-1234",
            "Fiat",
            "Branco",
            "Uno",
            50,
            2022,
            "path"
        )
        { FuelType = EFuelType.Gasoline, GroupId = Guid.NewGuid() };

        Rental rental = new(
            DateTimeOffset.UtcNow.AddDays(-5),
            DateTimeOffset.UtcNow.AddDays(1),
            1000m
        )
        {
            Id = rentalId,
            PricingPlan = pricingPlan,
            SelectedPlanType = EPricingPlanType.Daily,
            Vehicle = vehicle
        };

        this.repositoryRentalMock
            .Setup(r => r.GetByIdAsync(rentalId))
            .ReturnsAsync(rental);

        ReturnRentalRequest request = new(rentalId, 1100m, EFuelLevel.Full);

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<RentalReturn>(),
                CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        // Act
        Result<ReturnRentalResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(1600m, rental.FinalPrice);
        Assert.AreEqual(ERentalStatus.Completed, rental.Status);

        this.repositoryRentalReturnMock
            .Verify(r => r.AddAsync(
            It.Is<RentalReturn>(rr =>
                rr.RentalId == rentalId &&
                rr.FinalPrice == 1600m &&
                rr.EndKm == 1100m
        )), Times.Once);

        this.unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }
}