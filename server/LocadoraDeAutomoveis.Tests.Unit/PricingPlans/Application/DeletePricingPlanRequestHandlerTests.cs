using LocadoraDeAutomoveis.Application.PricingPlans.Commands.Delete;
using LocadoraDeAutomoveis.Domain.PricingPlans;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.PricingPlans.Application;

[TestClass]
[TestCategory("PricingPlan Application - Unit Tests")]
public class DeletePricingPlanRequestHandlerTests
{
    private DeletePricingPlanRequestHandler handler = null!;

    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryPricingPlan> repositoryPricingPlanMock = null!;
    private Mock<ILogger<DeletePricingPlanRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryPricingPlanMock = new Mock<IRepositoryPricingPlan>();
        this.loggerMock = new Mock<ILogger<DeletePricingPlanRequestHandler>>();

        this.handler = new DeletePricingPlanRequestHandler(
            this.unitOfWorkMock.Object,
            this.repositoryPricingPlanMock.Object,
            this.loggerMock.Object
        );
    }

    #region DeletePricingPlan Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldDeletePricingPlan_Successfully()
    {
        // Arrange
        Guid pricingPlanId = Guid.NewGuid();
        DeletePricingPlanRequest request = new(pricingPlanId);

        PricingPlan pricingPlan = new(
            new DailyPlanProps(90m, 1.5m),
            new ControlledPlanProps(140m, 2, 90),
            new FreePlanProps(190m)
        )
        { Id = pricingPlanId };

        this.repositoryPricingPlanMock
            .Setup(r => r.GetByIdAsync(pricingPlanId))
            .ReturnsAsync(pricingPlan);

        this.repositoryPricingPlanMock
            .Setup(r => r.DeleteAsync(request.Id))
            .Verifiable();

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync())
            .Verifiable();

        // Act
        Result<DeletePricingPlanResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.repositoryPricingPlanMock
            .Verify(r => r.GetByIdAsync(pricingPlanId),
            Times.Once);

        this.repositoryPricingPlanMock
            .Verify(r => r.DeleteAsync(pricingPlanId),
            Times.Once);

        this.unitOfWorkMock.Verify(c => c.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
