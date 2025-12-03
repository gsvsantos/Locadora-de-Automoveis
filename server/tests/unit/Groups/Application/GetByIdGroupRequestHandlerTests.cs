using LocadoraDeAutomoveis.Application.Groups.Commands.GetAll;
using LocadoraDeAutomoveis.Application.Groups.Commands.GetById;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Groups.Application;

[TestClass]
[TestCategory("Group Application - Unit Tests")]
public sealed class GetByIdGroupRequestHandlerTests : UnitTestBase
{
    private GetByIdGroupRequestHandler handler = null!;

    private Mock<IRepositoryGroup> repositoryGroupMock = null!;
    private Mock<ILogger<GetByIdGroupRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryGroupMock = new Mock<IRepositoryGroup>();
        this.loggerMock = new Mock<ILogger<GetByIdGroupRequestHandler>>();

        this.handler = new GetByIdGroupRequestHandler(
            this.mapper,
            this.repositoryGroupMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetGroupById Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetGroupById_Successfuly()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        GetByIdGroupRequest request = new(groupId);

        Group group = new("Grupo")
        { Id = groupId };

        this.repositoryGroupMock
            .Setup(repo => repo.GetByIdAsync(groupId))
            .ReturnsAsync(group);

        // Act
        Result<GetByIdGroupResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        GroupDto dto = result.Value.Group;

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(group.Id, dto.Id);
        Assert.AreEqual(group.Name, dto.Name);
    }
    #endregion
}
