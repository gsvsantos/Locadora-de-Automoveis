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
            new DailyBilling(90m, 1.5m),
            new ControlledBilling(140m, 90),
            new FreeBilling(190m)
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
        Assert.AreEqual(BillingPlan.Daily.DailyRate, dto.DailyBilling.DailyRate);
        Assert.AreEqual(BillingPlan.Daily.PricePerKm, dto.DailyBilling.PricePerKm);
        Assert.AreEqual(BillingPlan.Controlled.DailyRate, dto.ControlledBilling.DailyRate);
        Assert.AreEqual(BillingPlan.Controlled.PricePerKmExtrapolated, dto.ControlledBilling.PricePerKmExtrapolated);
        Assert.AreEqual(BillingPlan.Free.FixedRate, dto.FreeBilling.FixedRate);
        Assert.AreEqual(BillingPlan.GroupId, dto.Group.Id);
    }
    #endregion
}
