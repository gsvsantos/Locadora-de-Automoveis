using LocadoraDeAutomoveis.Application.PricingPlans.Commands.Create;
using LocadoraDeAutomoveis.Application.PricingPlans.Commands.Update;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.PricingPlans;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.PricingPlans.Application;

[TestClass]
[TestCategory("PricingPlan Application - Unit Tests")]
public sealed class UpdatePricingPlanRequestHandlerTests
{
    private UpdatePricingPlanRequestHandler handler = null!;

    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryPricingPlan> repositoryPricingPlanMock = null!;
    private Mock<IRepositoryGroup> repositoryGroupMock = null!;
    private Mock<IValidator<PricingPlan>> validatorMock = null!;
    private Mock<ILogger<UpdatePricingPlanRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryPricingPlanMock = new Mock<IRepositoryPricingPlan>();
        this.repositoryGroupMock = new Mock<IRepositoryGroup>();
        this.validatorMock = new Mock<IValidator<PricingPlan>>();
        this.loggerMock = new Mock<ILogger<UpdatePricingPlanRequestHandler>>();

        this.handler = new UpdatePricingPlanRequestHandler(
            this.unitOfWorkMock.Object,
            this.repositoryPricingPlanMock.Object,
            this.repositoryGroupMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region UpdatePricingPlan Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldUpdatePricingPlan_Successfully()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Group group = new("Grupo")
        { Id = groupId };

        Guid pricingPlanId = Guid.NewGuid();
        PricingPlan pricingPlan = new(
            "SuuuV Plan",
            new DailyPlanProps(90m, 1.5m),
            new ControlledPlanProps(140m, 90),
            new FreePlanProps(190m)
        )
        { Id = pricingPlanId };
        pricingPlan.AssociateGroup(group);

        this.repositoryPricingPlanMock
            .Setup(r => r.GetByIdAsync(pricingPlanId))
            .ReturnsAsync(pricingPlan);

        Guid group2Id = Guid.NewGuid();
        Group group2 = new("Grupo 2")
        { Id = groupId };
        this.repositoryGroupMock
            .Setup(r => r.GetByIdAsync(group2Id))
            .ReturnsAsync(group2);

        UpdatePricingPlanRequest request = new(
            pricingPlanId,
            group2Id,
            new DailyPlanDto(100m, 2m),
            new ControlledPlanDto(150m, 100),
            new FreePlanDto(200m)
        );

        PricingPlan updatedPricingPlan = new(
            "SUV Plan",
            request.DailyPlan.ToProps(),
            request.ControlledPlan.ToProps(),
            request.FreePlan.ToProps()
        );
        updatedPricingPlan.AssociateGroup(group2);

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<PricingPlan>(p =>
                    p.DailyPlan == updatedPricingPlan.DailyPlan &&
                    p.ControlledPlan == updatedPricingPlan.ControlledPlan &&
                    p.FreePlan == updatedPricingPlan.FreePlan
                    ), CancellationToken.None
                ))
            .ReturnsAsync(new ValidationResult());

        this.repositoryPricingPlanMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([pricingPlan]);

        // Act
        Result<UpdatePricingPlanResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.Is<PricingPlan>(p =>
                    p.DailyPlan == updatedPricingPlan.DailyPlan &&
                    p.ControlledPlan == updatedPricingPlan.ControlledPlan &&
                    p.FreePlan == updatedPricingPlan.FreePlan
                    ), CancellationToken.None
                ), Times.Once
            );

        this.repositoryPricingPlanMock
            .Verify(r => r.GetAllAsync(), Times.Once);

        this.repositoryPricingPlanMock
            .Verify(r => r.UpdateAsync(
                pricingPlanId,
                It.Is<PricingPlan>(p =>
                    p.DailyPlan == updatedPricingPlan.DailyPlan &&
                    p.ControlledPlan == updatedPricingPlan.ControlledPlan &&
                    p.FreePlan == updatedPricingPlan.FreePlan
                    )), Times.Once
            );

        this.unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
