using LocadoraDeAutomoveis.Application.Groups.Commands.Update;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Groups.Application;

[TestClass]
[TestCategory("Group Application - Unit Tests")]
public sealed class UpdateGroupRequestHandlerTests : UnitTestBase
{
    private UpdateGroupRequestHandler handler = null!;

    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryGroup> repositoryGroupMock = null!;
    private Mock<IValidator<Group>> validatorMock = null!;
    private Mock<ILogger<UpdateGroupRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryGroupMock = new Mock<IRepositoryGroup>();
        this.validatorMock = new Mock<IValidator<Group>>();
        this.loggerMock = new Mock<ILogger<UpdateGroupRequestHandler>>();

        this.handler = new UpdateGroupRequestHandler(
            this.unitOfWorkMock.Object,
            this.mapper,
            this.repositoryGroupMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region UpdateGroup Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldUpdateGroup_Successfully()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        UpdateGroupRequest request = new(groupId, "Group Editado");

        Group group = new("Grupo")
        { Id = groupId };

        this.repositoryGroupMock
            .Setup(r => r.GetByIdAsync(groupId))
            .ReturnsAsync(group);

        Group updatedGroup = new(request.Name);

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<Group>(g =>
                    g.Name == updatedGroup.Name
                    ), CancellationToken.None
                ))
            .ReturnsAsync(new ValidationResult());

        this.repositoryGroupMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([group]);

        this.repositoryGroupMock
            .Setup(r => r.UpdateAsync(request.Id, updatedGroup))
            .Verifiable();

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync()
            ).Verifiable();

        // Act
        Result<UpdateGroupResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.repositoryGroupMock
            .Verify(r => r.GetByIdAsync(groupId),
            Times.Once);

        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.Is<Group>(g =>
                    g.Name == updatedGroup.Name
                    ), CancellationToken.None
                ), Times.Once
            );

        this.repositoryGroupMock
            .Verify(r => r.GetAllAsync(),
            Times.Once);

        this.repositoryGroupMock
            .Verify(r => r.UpdateAsync(
                request.Id,
                It.Is<Group>(g =>
                    g.Name == request.Name
                    )
                ), Times.Once
            );

        this.unitOfWorkMock.Verify(c => c.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(groupId, result.Value.Id);
    }
    #endregion
}
