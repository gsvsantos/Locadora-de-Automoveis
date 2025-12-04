using LocadoraDeAutomoveis.Application.BillingPlans.Commands.Create;
using LocadoraDeAutomoveis.Application.BillingPlans.Commands.Update;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.BillingPlans.Application;

[TestClass]
[TestCategory("BillingPlan Application - Unit Tests")]
public sealed class UpdateBillingPlanRequestHandlerTests : UnitTestBase
{
    private UpdateBillingPlanRequestHandler handler = null!;

    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryBillingPlan> repositoryBillingPlanMock = null!;
    private Mock<IRepositoryGroup> repositoryGroupMock = null!;
    private Mock<IValidator<BillingPlan>> validatorMock = null!;
    private Mock<ILogger<UpdateBillingPlanRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryBillingPlanMock = new Mock<IRepositoryBillingPlan>();
        this.repositoryGroupMock = new Mock<IRepositoryGroup>();
        this.validatorMock = new Mock<IValidator<BillingPlan>>();
        this.loggerMock = new Mock<ILogger<UpdateBillingPlanRequestHandler>>();

        this.handler = new UpdateBillingPlanRequestHandler(
            this.unitOfWorkMock.Object,
            this.mapper,
            this.repositoryBillingPlanMock.Object,
            this.repositoryGroupMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region UpdateBillingPlan Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldUpdateBillingPlan_Successfully()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Group group = new("Grupo")
        { Id = groupId };

        Guid BillingPlanId = Guid.NewGuid();
        BillingPlan BillingPlan = new(
            "SuuuV Plan",
            new DailyBilling(90m, 1.5m),
            new ControlledBilling(140m, 90),
            new FreeBilling(190m)
        )
        { Id = BillingPlanId };
        BillingPlan.AssociateGroup(group);

        this.repositoryBillingPlanMock
            .Setup(r => r.GetByIdAsync(BillingPlanId))
            .ReturnsAsync(BillingPlan);

        Guid group2Id = Guid.NewGuid();
        Group group2 = new("Grupo 2")
        { Id = groupId };
        this.repositoryGroupMock
            .Setup(r => r.GetByIdAsync(group2Id))
            .ReturnsAsync(group2);

        UpdateBillingPlanRequest request = new(
            BillingPlanId,
            group2Id,
            new DailyPlanDto(100m, 2m),
            new ControlledPlanDto(150m, 100),
            new FreePlanDto(200m)
        );

        BillingPlan updatedBillingPlan = new(
            "SUV Plan",
            request.DailyPlan.ToProps(),
            request.ControlledPlan.ToProps(),
            request.FreePlan.ToProps()
        );
        updatedBillingPlan.AssociateGroup(group2);

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<BillingPlan>(p =>
                    p.Daily == updatedBillingPlan.Daily &&
                    p.Controlled == updatedBillingPlan.Controlled &&
                    p.Free == updatedBillingPlan.Free
                    ), CancellationToken.None
                ))
            .ReturnsAsync(new ValidationResult());

        this.repositoryBillingPlanMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([BillingPlan]);

        // Act
        Result<UpdateBillingPlanResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.Is<BillingPlan>(p =>
                    p.Daily == updatedBillingPlan.Daily &&
                    p.Controlled == updatedBillingPlan.Controlled &&
                    p.Free == updatedBillingPlan.Free
                    ), CancellationToken.None
                ), Times.Once
            );

        this.repositoryBillingPlanMock
            .Verify(r => r.GetAllAsync(), Times.Once);

        this.repositoryBillingPlanMock
            .Verify(r => r.UpdateAsync(
                BillingPlanId,
                It.Is<BillingPlan>(p =>
                    p.Daily == updatedBillingPlan.Daily &&
                    p.Controlled == updatedBillingPlan.Controlled &&
                    p.Free == updatedBillingPlan.Free
                    )), Times.Once
            );

        this.unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
