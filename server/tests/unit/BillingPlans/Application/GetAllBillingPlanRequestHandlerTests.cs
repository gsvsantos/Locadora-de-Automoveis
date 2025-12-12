using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Application.BillingPlans.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.BillingPlans.Application;

[TestClass]
[TestCategory("BillingPlan Application - Unit Tests")]
public sealed class GetAllBillingPlanRequestHandlerTests : UnitTestBase
{
    private GetAllBillingPlanRequestHandler handler = null!;

    private Mock<IRepositoryBillingPlan> repositoryBillingPlanMock = null!;
    private Mock<ILogger<GetAllBillingPlanRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryBillingPlanMock = new Mock<IRepositoryBillingPlan>();
        this.loggerMock = new Mock<ILogger<GetAllBillingPlanRequestHandler>>();

        this.handler = new GetAllBillingPlanRequestHandler(
            this.mapper,
            this.repositoryBillingPlanMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetAllBillingPlans Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetAllBillingPlans_Successfully()
    {
        // Arrange
        RandomGenerator random = new();
        List<BillingPlan> BillingPlans = Builder<BillingPlan>.CreateListOfSize(10).All()
            .Do(v => v.Name = random.NextString(1, 5))
            .Do(v => v.Daily = new(random.Decimal(), random.Decimal()))
            .Do(v => v.Controlled = new(random.Decimal(), random.Int()))
            .Do(v => v.Free = new(random.Decimal()))
            .Do(v => v.AssociateGroup(new($"group-{Guid.NewGuid()}")))
            .Build().ToList();

        this.repositoryBillingPlanMock
            .Setup(r => r.GetAllAsync(true))
            .ReturnsAsync(BillingPlans);

        GetAllBillingPlanRequest request = new(null, null);

        // Act
        Result<GetAllBillingPlanResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        List<BillingPlanDto> BillingPlansDto = [.. result.Value.BillingPlans];

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(10, result.Value.Quantity);
        Assert.AreEqual(BillingPlans.Count, BillingPlansDto.Count);

        for (int i = 0; i < BillingPlans.Count; i++)
        {
            for (int j = 0; j < BillingPlansDto.Count; j++)
            {
                if (BillingPlans[i].Id == BillingPlansDto[j].Id)
                {
                    Assert.AreEqual(BillingPlans[i].Name, BillingPlans[j].Name);
                    Assert.AreEqual(BillingPlans[i].Daily, BillingPlans[j].Daily);
                    Assert.AreEqual(BillingPlans[i].Controlled, BillingPlans[j].Controlled);
                    Assert.AreEqual(BillingPlans[i].Free, BillingPlans[j].Free);
                    Assert.AreEqual(BillingPlans[i].GroupId, BillingPlans[j].GroupId);
                }
            }
        }
    }

    [TestMethod]
    public void Handler_ShouldGetFiveBillingPlan_Successfully()
    {
        // Arrange
        RandomGenerator random = new();
        List<BillingPlan> BillingPlans = Builder<BillingPlan>.CreateListOfSize(10).All()
            .Do(v => v.Name = random.NextString(1, 5))
            .Do(v => v.Daily = new(random.Decimal(), random.Decimal()))
            .Do(v => v.Controlled = new(random.Decimal(), random.Int()))
            .Do(v => v.Free = new(random.Decimal()))
            .Do(v => v.AssociateGroup(new($"group-{Guid.NewGuid()}")))
            .Build().ToList();

        this.repositoryBillingPlanMock
            .Setup(r => r.GetAllAsync(5))
            .ReturnsAsync([.. BillingPlans.Take(5)]);

        GetAllBillingPlanRequest request = new(5, null);

        // Act
        Result<GetAllBillingPlanResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        List<BillingPlanDto> BillingPlansDto = [.. result.Value.BillingPlans];

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(10, BillingPlans.Count);
        Assert.AreEqual(5, result.Value.Quantity);
        Assert.AreEqual(5, BillingPlansDto.Count);

        for (int i = 0; i < BillingPlans.Count; i++)
        {
            for (int j = 0; j < BillingPlansDto.Count; j++)
            {
                if (BillingPlans[i].Id == BillingPlansDto[j].Id)
                {
                    Assert.AreEqual(BillingPlans[i].Name, BillingPlans[j].Name);
                    Assert.AreEqual(BillingPlans[i].Daily, BillingPlans[j].Daily);
                    Assert.AreEqual(BillingPlans[i].Free, BillingPlans[j].Free);
                    Assert.AreEqual(BillingPlans[i].Controlled, BillingPlans[j].Controlled);
                    Assert.AreEqual(BillingPlans[i].GroupId, BillingPlans[j].GroupId);
                }
            }
        }
    }
    #endregion
}
