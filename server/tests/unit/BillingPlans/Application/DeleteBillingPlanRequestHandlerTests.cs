using LocadoraDeAutomoveis.Application.BillingPlans.Commands.Delete;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.BillingPlans.Application;

[TestClass]
[TestCategory("BillingPlan Application - Unit Tests")]
public class DeleteBillingPlanRequestHandlerTests
{
    private DeleteBillingPlanRequestHandler handler = null!;

    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryBillingPlan> repositoryBillingPlanMock = null!;
    private Mock<IRepositoryRental> repositoryRentalMock = null!;
    private Mock<ILogger<DeleteBillingPlanRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryBillingPlanMock = new Mock<IRepositoryBillingPlan>();
        this.repositoryRentalMock = new Mock<IRepositoryRental>();
        this.loggerMock = new Mock<ILogger<DeleteBillingPlanRequestHandler>>();

        this.handler = new DeleteBillingPlanRequestHandler(
            this.unitOfWorkMock.Object,
            this.repositoryBillingPlanMock.Object,
            this.repositoryRentalMock.Object,
            this.loggerMock.Object
        );
    }

    #region DeleteBillingPlan Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldDeleteBillingPlan_Successfully()
    {
        // Arrange
        Guid BillingPlanId = Guid.NewGuid();
        DeleteBillingPlanRequest request = new(BillingPlanId);

        BillingPlan BillingPlan = new(
            "SUV Plan",
            new DailyPlanProps(90m, 1.5m),
            new ControlledPlanProps(140m, 90),
            new FreePlanProps(190m)
        )
        { Id = BillingPlanId };

        this.repositoryBillingPlanMock
            .Setup(r => r.GetByIdAsync(BillingPlanId))
            .ReturnsAsync(BillingPlan);

        this.repositoryBillingPlanMock
            .Setup(r => r.DeleteAsync(request.Id))
            .Verifiable();

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync())
            .Verifiable();

        // Act
        Result<DeleteBillingPlanResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.repositoryBillingPlanMock
            .Verify(r => r.GetByIdAsync(BillingPlanId),
            Times.Once);

        this.repositoryBillingPlanMock
            .Verify(r => r.DeleteAsync(BillingPlanId),
            Times.Once);

        this.unitOfWorkMock.Verify(c => c.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
