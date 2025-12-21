using LocadoraDeAutomoveis.Application.Clients.Commands.Create;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Clients.Application;

[TestClass]
[TestCategory("Client Application - Unit Tests")]
public sealed class CreateClientRequestHandlerTests : UnitTestBase
{
    private CreateClientRequestHandler handler = null!;

    private const string _userName = "Clebinho";
    private const string _fullName = "Clebinho Da Silva";
    private const string _email = "cleber@dasilva.net";
    private const string _phoneNumber = "(99) 99999-9999";

    private Mock<UserManager<User>> userManagerMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryClient> repositoryClientMock = null!;
    private Mock<ITenantProvider> tenantProviderMock = null!;
    private Mock<IUserContext> userContextMock = null!;
    private Mock<IAuthEmailService> emailServiceMock = null!;
    private Mock<IValidator<Client>> validatorMock = null!;
    private Mock<ILogger<CreateClientRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!
        );

        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryClientMock = new Mock<IRepositoryClient>();
        this.tenantProviderMock = new Mock<ITenantProvider>();
        this.userContextMock = new Mock<IUserContext>();
        this.emailServiceMock = new Mock<IAuthEmailService>();
        this.validatorMock = new Mock<IValidator<Client>>();
        this.loggerMock = new Mock<ILogger<CreateClientRequestHandler>>();

        this.handler = new CreateClientRequestHandler(
            this.userManagerMock.Object,
            this.unitOfWorkMock.Object,
            this.mapper,
            this.repositoryClientMock.Object,
            this.tenantProviderMock.Object,
            this.userContextMock.Object,
            this.emailServiceMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region CreateClient Tests (Happy Path)
    [TestMethod]
    public async Task Handler_ShouldCreateClient_Successfully()
    {
        // Arrange
        Guid tenantId = Guid.NewGuid();
        this.tenantProviderMock
            .Setup(t => t.GetTenantId())
            .Returns(tenantId);

        Guid currentUserId = Guid.NewGuid();
        this.userContextMock
            .Setup(c => c.GetUserId())
            .Returns(currentUserId);

        User currentUser = new()
        {
            Id = currentUserId,
            UserName = _userName,
            FullName = _fullName,
            Email = _email,
            PhoneNumber = _phoneNumber,
            TenantId = tenantId
        };

        this.userManagerMock
            .Setup(u => u.FindByIdAsync(It.Is<string>(id => id == currentUserId.ToString())))
            .ReturnsAsync(currentUser);

        CreateClientRequest request = new(
            "Ricardo",
            "ricardo@gmail.com",
            "(51) 90000-0001",
            "RS",
            "Carazinho",
            "Marcondes",
            "Edi Marcondes",
            33,
            EClientType.Individual,
            "000.000.000-01"
        );

        this.userManagerMock
            .Setup(u => u.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Success);

        this.userManagerMock
            .Setup(u => u.AddToRoleAsync(It.IsAny<User>(), "Client"))
            .ReturnsAsync(IdentityResult.Success);

        this.userManagerMock
            .Setup(u => u.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Success);

        this.validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        this.repositoryClientMock
            .Setup(r => r.ExistsByDocumentAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        this.repositoryClientMock
            .Setup(r => r.AddAsync(It.IsAny<Client>()))
            .Returns(Task.CompletedTask);

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync())
            .Returns(Task.CompletedTask);

        this.userManagerMock
            .Setup(u => u.GeneratePasswordResetTokenAsync(It.IsAny<User>()))
            .ReturnsAsync("token");

        this.emailServiceMock
            .Setup(s => s.ScheduleClientInvitation(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            ))
            .Returns(Task.CompletedTask);

        // Act
        Result<CreateClientResponse> result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        this.userManagerMock.Verify(
            u => u.FindByIdAsync(It.Is<string>(id => id == currentUserId.ToString())),
            Times.Once
        );

        this.validatorMock.Verify(
            v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()),
            Times.Once
        );

        this.repositoryClientMock.Verify(
            r => r.ExistsByDocumentAsync(It.Is<string>(doc => doc == request.Document)),
            Times.Once
        );

        this.repositoryClientMock.Verify(
            r => r.AddAsync(It.IsAny<Client>()),
            Times.Once
        );

        this.unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}