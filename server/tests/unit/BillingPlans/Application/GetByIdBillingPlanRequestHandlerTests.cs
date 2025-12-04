using LocadoraDeAutomoveis.Application.BillingPlans.Commands.GetAll;
using LocadoraDeAutomoveis.Application.BillingPlans.Commands.GetById;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.BillingPlans.Application;

[TestClass]
[TestCategory("BillingPlan Application - Unit Tests")]
public sealed class GetByIdBillingPlanRequestHandlerTests : UnitTestBase
{
    private GetByIdBillingPlanRequestHandler handler = null!;

    private Mock<IRepositoryBillingPlan> repositoryBillingPlanMock = null!;
    private Mock<ILogger<GetByIdBillingPlanRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryBillingPlanMock = new Mock<IRepositoryBillingPlan>();
        this.loggerMock = new Mock<ILogger<GetByIdBillingPlanRequestHandler>>();

        this.handler = new GetByIdBillingPlanRequestHandler(
            this.mapper,
            this.repositoryBillingPlanMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetBillingPlanById Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetBillingPlansById_Successfuly()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Group group = new("Grupo")
        { Id = groupId };

        Guid BillingPlanId = Guid.NewGuid();
        GetByIdBillingPlanRequest request = new(BillingPlanId);
        BillingPlan BillingPlan = new(
            $"{group.Name} - Billing Plans",
            new DailyPlanProps(90m, 1.5m),
            new ControlledPlanProps(140m, 90),
            new FreePlanProps(190m)
        )
        { Id = BillingPlanId };
        BillingPlan.AssociateGroup(group);

        this.repositoryBillingPlanMock
            .Setup(r => r.GetByIdAsync(BillingPlanId))
            .ReturnsAsync(BillingPlan);

        // Act
        Result<GetByIdBillingPlanResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        BillingPlanDto dto = result.Value.BillingPlan;

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(BillingPlan.Id, dto.Id);
        Assert.AreEqual(BillingPlan.Name, dto.Name);
        Assert.AreEqual(BillingPlan.DailyPlan.DailyRate, dto.DailyPlan.DailyRate);
        Assert.AreEqual(BillingPlan.DailyPlan.PricePerKm, dto.DailyPlan.PricePerKm);
        Assert.AreEqual(BillingPlan.ControlledPlan.DailyRate, dto.ControlledPlan.DailyRate);
        Assert.AreEqual(BillingPlan.ControlledPlan.PricePerKmExtrapolated, dto.ControlledPlan.PricePerKmExtrapolated);
        Assert.AreEqual(BillingPlan.FreePlan.FixedRate, dto.FreePlan.FixedRate);
        Assert.AreEqual(BillingPlan.GroupId, dto.GroupId);
    }
    #endregion
}
