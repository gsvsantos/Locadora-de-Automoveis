using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Application.Groups.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Groups.Application;

[TestClass]
[TestCategory("Group Application - Unit Tests")]
public sealed class GetAllGroupRequestHandlerTests : UnitTestBase
{
    private GetAllGroupRequestHandler handler = null!;

    private Mock<IRepositoryGroup> repositoryGroupMock = null!;
    private Mock<ILogger<GetAllGroupRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryGroupMock = new Mock<IRepositoryGroup>();
        this.loggerMock = new Mock<ILogger<GetAllGroupRequestHandler>>();

        this.handler = new GetAllGroupRequestHandler(
            this.mapper,
            this.repositoryGroupMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetAllEmployees Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetAllEmployees_Successfully()
    {
        // Arrange
        List<Group> groups = Builder<Group>.CreateListOfSize(10).Build().ToList();

        this.repositoryGroupMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(groups);

        GetAllGroupRequest request = new(null);

        // Act
        Result<GetAllGroupResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        List<GroupDto> groupsDto = [.. result.Value.Groups];

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(10, result.Value.Quantity);
        Assert.AreEqual(groups.Count, groupsDto.Count);

        for (int i = 0; i < groups.Count; i++)
        {
            for (int j = 0; j < groupsDto.Count; j++)
            {
                if (groups[i].Id == groupsDto[j].Id)
                {
                    Assert.AreEqual(groups[i].Name, groupsDto[j].Name);
                }
            }
        }
    }

    [TestMethod]
    public void Handler_ShouldGetFiveEmployees_Successfully()
    {
        // Arrange
        List<Group> groups = Builder<Group>.CreateListOfSize(10).Build().ToList();

        this.repositoryGroupMock
            .Setup(r => r.GetAllAsync(5))
            .ReturnsAsync([.. groups.Take(5)]);

        GetAllGroupRequest request = new(5);

        // Act
        Result<GetAllGroupResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        List<GroupDto> groupsDto = [.. result.Value.Groups];

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(10, groups.Count);
        Assert.AreEqual(5, result.Value.Quantity);
        Assert.AreEqual(5, groupsDto.Count);

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < groupsDto.Count; j++)
            {
                if (groups[i].Id == groupsDto[j].Id)
                {
                    Assert.AreEqual(groups[i].Name, groupsDto[j].Name);
                }
            }
        }
    }
    #endregion
}
