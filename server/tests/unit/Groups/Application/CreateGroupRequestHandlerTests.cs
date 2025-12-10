using LocadoraDeAutomoveis.Application.Groups.Commands.Create;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Groups.Application;

[TestClass]
[TestCategory("Group Application - Unit Tests")]
public sealed class CreateGroupRequestHandlerTests : UnitTestBase
{
    private CreateGroupRequestHandler handler = null!;

    private const string _userName = "Clebinho";
    private const string _fullName = "Clebinho Da Silva";
    private const string _email = "cleber@dasilva.net";
    private const string _phoneNumber = "(99) 99999-9999";

    private Mock<UserManager<User>> userManagerMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryGroup> repositoryGroupMock = null!;
    private Mock<ITenantProvider> tenantProviderMock = null!;
    private Mock<IUserContext> userContextMock = null!;
    private Mock<IValidator<Group>> validatorMock = null!;
    private Mock<ILogger<CreateGroupRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!
        );

        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryGroupMock = new Mock<IRepositoryGroup>();
        this.tenantProviderMock = new Mock<ITenantProvider>();
        this.userContextMock = new Mock<IUserContext>();
        this.validatorMock = new Mock<IValidator<Group>>();
        this.loggerMock = new Mock<ILogger<CreateGroupRequestHandler>>();

        this.handler = new CreateGroupRequestHandler(
            this.userManagerMock.Object,
            this.unitOfWorkMock.Object,
            this.mapper,
            this.repositoryGroupMock.Object,
            this.tenantProviderMock.Object,
            this.userContextMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region CreateGroup Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldCreateGroup_Successfully()
    {
        // Arrange
        Guid tenantId = Guid.NewGuid();
        this.tenantProviderMock
            .Setup(t => t.GetTenantId())
            .Returns(tenantId);

        Guid userId = Guid.NewGuid();
        User user = new()
        {
            Id = userId,
            UserName = _userName,
            FullName = _fullName,
            Email = _email,
            PhoneNumber = _phoneNumber,
        };

        CreateGroupRequest request = new("Fusca");

        this.userContextMock
            .Setup(t => t.GetUserId())
            .Returns(userId);

        this.userManagerMock
            .Setup(u => u.FindByIdAsync(this.userContextMock.Object.GetUserId().ToString()))
            .ReturnsAsync(user);

        Group group = new(request.Name);

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<Group>(g =>
                    g.Name == request.Name
                    ), CancellationToken.None
                ))
            .ReturnsAsync(new ValidationResult());

        this.repositoryGroupMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        // Act
        Result<CreateGroupResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.Is<Group>(g =>
                    g.Name == request.Name
                    ), CancellationToken.None
                ), Times.Once
            );

        this.repositoryGroupMock
            .Verify(r => r.GetAllAsync(), Times.Once);

        this.repositoryGroupMock
            .Verify(r => r.AddAsync(
                It.Is<Group>(g =>
                    g.Name == request.Name
                    )), Times.Once
            );

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
