using LocadoraDeAutomoveis.Application.BillingPlans.Commands.Create;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Tests.Unit.Shared;
using Microsoft.Extensions.Caching.Distributed;

namespace LocadoraDeAutomoveis.Tests.Unit.BillingPlans.Application;

[TestClass]
[TestCategory("BillingPlan Application - Unit Tests")]
public sealed class CreateBillingPlanRequestHandlerTests : UnitTestBase
{
    private CreateBillingPlanRequestHandler handler = null!;

    private const string _userName = "Clebinho";
    private const string _fullName = "Clebinho Da Silva";
    private const string _email = "cleber@dasilva.net";
    private const string _phoneNumber = "(99) 99999-9999";

    private Mock<UserManager<User>> userManagerMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryBillingPlan> repositoryBillingPlanMock = null!;
    private Mock<IRepositoryGroup> repositoryGroupMock = null!;
    private Mock<IDistributedCache> cacheMock = null!;
    private Mock<ITenantProvider> tenantProviderMock = null!;
    private Mock<IUserContext> userContextMock = null!;
    private Mock<IValidator<BillingPlan>> validatorMock = null!;
    private Mock<ILogger<CreateBillingPlanRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!
        );

        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryBillingPlanMock = new Mock<IRepositoryBillingPlan>();
        this.repositoryGroupMock = new Mock<IRepositoryGroup>();
        this.cacheMock = new Mock<IDistributedCache>();
        this.tenantProviderMock = new Mock<ITenantProvider>();
        this.userContextMock = new Mock<IUserContext>();
        this.validatorMock = new Mock<IValidator<BillingPlan>>();
        this.loggerMock = new Mock<ILogger<CreateBillingPlanRequestHandler>>();

        this.handler = new CreateBillingPlanRequestHandler(
            this.userManagerMock.Object,
            this.unitOfWorkMock.Object,
            this.mapper,
            this.repositoryBillingPlanMock.Object,
            this.repositoryGroupMock.Object,
            this.cacheMock.Object,
            this.tenantProviderMock.Object,
            this.userContextMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region CreateBillingPlan Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldCreateBillingPlan_Successfully()
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

        CreateBillingPlanRequest request = new(
            groupId,
            new DailyBillingDto(100m, 2m),
            new ControlledBillingDto(150m, 100),
            new FreeBillingDto(200m)
        );

        this.userContextMock
            .Setup(t => t.GetUserId())
            .Returns(userId);

        this.userManagerMock
            .Setup(u => u.FindByIdAsync(this.userContextMock.Object.GetUserId().ToString()))
            .ReturnsAsync(user);

        BillingPlan BillingPlan = new(
            "SUV Plan",
            request.DailyBilling.ToProps(),
            request.ControlledBilling.ToProps(),
            request.FreeBilling.ToProps()
        );

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<BillingPlan>(p =>
                    p.Daily == request.DailyBilling.ToProps() &&
                    p.Controlled == request.ControlledBilling.ToProps() &&
                    p.Free == request.FreeBilling.ToProps()
                    ), CancellationToken.None
                ))
            .ReturnsAsync(new ValidationResult());

        this.repositoryBillingPlanMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        // Act
        Result<CreateBillingPlanResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.Is<BillingPlan>(p =>
                    p.Daily == request.DailyBilling.ToProps() &&
                    p.Controlled == request.ControlledBilling.ToProps() &&
                    p.Free == request.FreeBilling.ToProps()
                    ), CancellationToken.None
                ), Times.Once
            );

        this.repositoryBillingPlanMock
            .Verify(r => r.GetAllAsync(), Times.Once);

        this.repositoryBillingPlanMock
            .Verify(r => r.AddAsync(
                It.Is<BillingPlan>(p =>
                    p.Daily == request.DailyBilling.ToProps() &&
                    p.Controlled == request.ControlledBilling.ToProps() &&
                    p.Free == request.FreeBilling.ToProps()
                    )), Times.Once
            );

        this.unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
