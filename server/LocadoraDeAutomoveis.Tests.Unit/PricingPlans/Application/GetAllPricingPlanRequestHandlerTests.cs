using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Application.PricingPlans.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.PricingPlans;

namespace LocadoraDeAutomoveis.Tests.Unit.PricingPlans.Application;

[TestClass]
[TestCategory("PricingPlan Application - Unit Tests")]
public sealed class GetAllPricingPlanRequestHandlerTests
{
    private GetAllPricingPlanRequestHandler handler = null!;

    private Mock<IRepositoryPricingPlan> repositoryPricingPlanMock;
    private Mock<ILogger<GetAllPricingPlanRequestHandler>> loggerMock;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryPricingPlanMock = new Mock<IRepositoryPricingPlan>();
        this.loggerMock = new Mock<ILogger<GetAllPricingPlanRequestHandler>>();

        this.handler = new GetAllPricingPlanRequestHandler(
            this.repositoryPricingPlanMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetAllPricingPlans Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetAllPricingPlans_Successfully()
    {
        // Arrange
        RandomGenerator random = new();
        List<PricingPlan> pricingPlans = Builder<PricingPlan>.CreateListOfSize(10).All()
            .Do(v => v.DailyPlan = new(random.Decimal(), random.Decimal()))
            .Do(v => v.ControlledPlan = new(random.Decimal(), random.Decimal(), random.Int()))
            .Do(v => v.FreePlan = new(random.Decimal()))
            .Do(v => v.AssociateGroup(new($"group-{Guid.NewGuid()}")))
            .Build().ToList();

        this.repositoryPricingPlanMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(pricingPlans);

        GetAllPricingPlanRequest request = new(null);

        // Act
        Result<GetAllPricingPlanResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        List<PricingPlanDto> pricingPlansDto = [.. result.Value.PricingPlans];

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(10, result.Value.Quantity);
        Assert.AreEqual(pricingPlans.Count, pricingPlansDto.Count);

        for (int i = 0; i < pricingPlans.Count; i++)
        {
            for (int j = 0; j < pricingPlansDto.Count; j++)
            {
                if (pricingPlans[i].Id == pricingPlansDto[j].Id)
                {
                    Assert.AreEqual(pricingPlans[i].DailyPlan, pricingPlans[j].DailyPlan);
                    Assert.AreEqual(pricingPlans[i].FreePlan, pricingPlans[j].FreePlan);
                    Assert.AreEqual(pricingPlans[i].ControlledPlan, pricingPlans[j].ControlledPlan);
                    Assert.AreEqual(pricingPlans[i].GroupId, pricingPlans[j].GroupId);
                }
            }
        }
    }

    [TestMethod]
    public void Handler_ShouldGetFivePricingPlan_Successfully()
    {
        // Arrange
        RandomGenerator random = new();
        List<PricingPlan> pricingPlans = Builder<PricingPlan>.CreateListOfSize(10).All()
            .Do(v => v.DailyPlan = new(random.Decimal(), random.Decimal()))
            .Do(v => v.ControlledPlan = new(random.Decimal(), random.Decimal(), random.Int()))
            .Do(v => v.FreePlan = new(random.Decimal()))
            .Do(v => v.AssociateGroup(new($"group-{Guid.NewGuid()}")))
            .Build().ToList();

        this.repositoryPricingPlanMock
            .Setup(r => r.GetAllAsync(5))
            .ReturnsAsync([.. pricingPlans.Take(5)]);

        GetAllPricingPlanRequest request = new(5);

        // Act
        Result<GetAllPricingPlanResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        List<PricingPlanDto> pricingPlansDto = [.. result.Value.PricingPlans];

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(10, pricingPlans.Count);
        Assert.AreEqual(5, result.Value.Quantity);
        Assert.AreEqual(5, pricingPlansDto.Count);

        for (int i = 0; i < pricingPlans.Count; i++)
        {
            for (int j = 0; j < pricingPlansDto.Count; j++)
            {
                if (pricingPlans[i].Id == pricingPlansDto[j].Id)
                {
                    Assert.AreEqual(pricingPlans[i].DailyPlan, pricingPlans[j].DailyPlan);
                    Assert.AreEqual(pricingPlans[i].FreePlan, pricingPlans[j].FreePlan);
                    Assert.AreEqual(pricingPlans[i].ControlledPlan, pricingPlans[j].ControlledPlan);
                    Assert.AreEqual(pricingPlans[i].GroupId, pricingPlans[j].GroupId);
                }
            }
        }
    }
    #endregion
}
