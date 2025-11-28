using LocadoraDeAutomoveis.Application.PricingPlans.Commands.GetAll;
using LocadoraDeAutomoveis.Application.PricingPlans.Commands.GetById;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.PricingPlans;

namespace LocadoraDeAutomoveis.Tests.Unit.PricingPlans.Application;

[TestClass]
[TestCategory("PricingPlan Application - Unit Tests")]
public sealed class GetByIdPricingPlanRequestHandlerTests
{
    private GetByIdPricingPlanRequestHandler handler = null!;

    private Mock<IRepositoryPricingPlan> repositoryPricingPlanMock = null!;
    private Mock<ILogger<GetByIdPricingPlanRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryPricingPlanMock = new Mock<IRepositoryPricingPlan>();
        this.loggerMock = new Mock<ILogger<GetByIdPricingPlanRequestHandler>>();

        this.handler = new GetByIdPricingPlanRequestHandler(
            this.repositoryPricingPlanMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetPricingPlanById Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetPricingPlansById_Successfuly()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Group group = new("Grupo")
        { Id = groupId };

        Guid pricingPlanId = Guid.NewGuid();
        GetByIdPricingPlanRequest request = new(pricingPlanId);
        PricingPlan pricingPlan = new(
            $"{group.Name} - Pricing Plans",
            new DailyPlanProps(90m, 1.5m),
            new ControlledPlanProps(140m, 90),
            new FreePlanProps(190m)
        )
        { Id = pricingPlanId };
        pricingPlan.AssociateGroup(group);

        this.repositoryPricingPlanMock
            .Setup(r => r.GetByIdAsync(pricingPlanId))
            .ReturnsAsync(pricingPlan);

        // Act
        Result<GetByIdPricingPlanResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        PricingPlanDto dto = result.Value.PricingPlan;

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(pricingPlan.Id, dto.Id);
        Assert.AreEqual(pricingPlan.Name, dto.Name);
        Assert.AreEqual(pricingPlan.DailyPlan.DailyRate, dto.DailyPlan.DailyRate);
        Assert.AreEqual(pricingPlan.DailyPlan.PricePerKm, dto.DailyPlan.PricePerKm);
        Assert.AreEqual(pricingPlan.ControlledPlan.DailyRate, dto.ControlledPlan.DailyRate);
        Assert.AreEqual(pricingPlan.ControlledPlan.PricePerKmExtrapolated, dto.ControlledPlan.PricePerKmExtrapolated);
        Assert.AreEqual(pricingPlan.FreePlan.FixedRate, dto.FreePlan.FixedRate);
        Assert.AreEqual(pricingPlan.GroupId, dto.GroupId);
    }
    #endregion
}
