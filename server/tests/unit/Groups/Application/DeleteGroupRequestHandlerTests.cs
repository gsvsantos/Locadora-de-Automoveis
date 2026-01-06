using LocadoraDeAutomoveis.Application.Groups.Commands.Delete;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using Microsoft.Extensions.Caching.Distributed;

namespace LocadoraDeAutomoveis.Tests.Unit.Groups.Application;

[TestClass]
[TestCategory("Group Application - Unit Tests")]
public sealed class DeleteGroupRequestHandlerTests
{
    private DeleteGroupRequestHandler handler = null!;

    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryGroup> repositoryGroupMock = null!;
    private Mock<IRepositoryVehicle> repositoryVehicleMock = null!;
    private Mock<IRepositoryBillingPlan> repositoryBillingPlanMock = null!;
    private Mock<IDistributedCache> cacheMock = null!;
    private Mock<ILogger<DeleteGroupRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryGroupMock = new Mock<IRepositoryGroup>();
        this.repositoryVehicleMock = new Mock<IRepositoryVehicle>();
        this.repositoryBillingPlanMock = new Mock<IRepositoryBillingPlan>();
        this.cacheMock = new Mock<IDistributedCache>();
        this.loggerMock = new Mock<ILogger<DeleteGroupRequestHandler>>();

        this.handler = new DeleteGroupRequestHandler(
            this.unitOfWorkMock.Object,
            this.repositoryGroupMock.Object,
            this.repositoryVehicleMock.Object,
            this.repositoryBillingPlanMock.Object,
            this.cacheMock.Object,
            this.loggerMock.Object
        );
    }

    #region DeleteGroup Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldDeleteGroup_Successfully()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        DeleteGroupRequest request = new(groupId);

        Group group = new("Grupo")
        { Id = groupId };

        this.repositoryGroupMock
            .Setup(r => r.GetByIdAsync(groupId))
            .ReturnsAsync(group);

        this.repositoryGroupMock
            .Setup(r => r.DeleteAsync(request.Id))
            .Verifiable();

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync()
            ).Verifiable();

        // Act
        Result<DeleteGroupResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.repositoryGroupMock
            .Verify(r => r.GetByIdAsync(groupId),
            Times.Once);

        this.repositoryGroupMock
            .Verify(r => r.DeleteAsync(groupId),
            Times.Once);

        this.unitOfWorkMock.Verify(c => c.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
