using LocadoraDeAutomoveis.Application.PricingPlans.Commands.Create;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.PricingPlans;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.PricingPlans.Application;

[TestClass]
[TestCategory("PricingPlan Application - Unit Tests")]
public sealed class CreatePricingPlanRequestHandlerTests
{
    private CreatePricingPlanRequestHandler handler = null!;

    private const string _userName = "Clebinho";
    private const string _fullName = "Clebinho Da Silva";
    private const string _email = "cleber@dasilva.net";
    private const string _phoneNumber = "(99) 99999-9999";

    private Mock<UserManager<User>> userManagerMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryPricingPlan> repositoryPricingPlanMock = null!;
    private Mock<IRepositoryGroup> repositoryGroupMock = null!;
    private Mock<ITenantProvider> tenantProviderMock = null!;
    private Mock<IUserContext> userContextMock = null!;
    private Mock<IValidator<PricingPlan>> validatorMock = null!;
    private Mock<ILogger<CreatePricingPlanRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!
        );

        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryPricingPlanMock = new Mock<IRepositoryPricingPlan>();
        this.repositoryGroupMock = new Mock<IRepositoryGroup>();
        this.tenantProviderMock = new Mock<ITenantProvider>();
        this.userContextMock = new Mock<IUserContext>();
        this.validatorMock = new Mock<IValidator<PricingPlan>>();
        this.loggerMock = new Mock<ILogger<CreatePricingPlanRequestHandler>>();

        this.handler = new CreatePricingPlanRequestHandler(
            this.userManagerMock.Object,
            this.unitOfWorkMock.Object,
            this.repositoryPricingPlanMock.Object,
            this.repositoryGroupMock.Object,
            this.tenantProviderMock.Object,
            this.userContextMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region CreatePricingPlan Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldCreatePricingPlan_Successfully()
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

        Guid groupId = Guid.NewGuid();
        Group group = new("Grupo")
        { Id = groupId };
        group.AssociateTenant(tenantId);
        group.AssociateUser(user);

        this.repositoryGroupMock
            .Setup(r => r.GetByIdAsync(groupId))
            .ReturnsAsync(group);

        CreatePricingPlanRequest request = new(
            groupId,
            new DailyPlanDto(100m, 2m),
            new ControlledPlanDto(150m, 3, 100),
            new FreePlanDto(200m)
        );

        this.userContextMock
            .Setup(t => t.GetUserId())
            .Returns(userId);

        this.userManagerMock
            .Setup(u => u.FindByIdAsync(this.userContextMock.Object.GetUserId().ToString()))
            .ReturnsAsync(user);

        PricingPlan pricingPlan = new(
            request.DailyPlan.ToProps(),
            request.ControlledPlan.ToProps(),
            request.FreePlan.ToProps()
        );

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<PricingPlan>(p =>
                    p.DailyPlan == request.DailyPlan.ToProps() &&
                    p.ControlledPlan == request.ControlledPlan.ToProps() &&
                    p.FreePlan == request.FreePlan.ToProps()
                    ), CancellationToken.None
                ))
            .ReturnsAsync(new ValidationResult());

        this.repositoryPricingPlanMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        // Act
        Result<CreatePricingPlanResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.Is<PricingPlan>(p =>
                    p.DailyPlan == request.DailyPlan.ToProps() &&
                    p.ControlledPlan == request.ControlledPlan.ToProps() &&
                    p.FreePlan == request.FreePlan.ToProps()
                    ), CancellationToken.None
                ), Times.Once
            );

        this.repositoryPricingPlanMock
            .Verify(r => r.GetAllAsync(), Times.Once);

        this.repositoryPricingPlanMock
            .Verify(r => r.AddAsync(
                It.Is<PricingPlan>(p =>
                    p.DailyPlan == request.DailyPlan.ToProps() &&
                    p.ControlledPlan == request.ControlledPlan.ToProps() &&
                    p.FreePlan == request.FreePlan.ToProps()
                    )), Times.Once
            );

        this.unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
