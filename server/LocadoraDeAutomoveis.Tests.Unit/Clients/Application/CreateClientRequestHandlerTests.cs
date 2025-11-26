using LocadoraDeAutomoveis.Application.Clients.Commands.Create;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Clients.Application;

[TestClass]
[TestCategory("Client Application - Unit Tests")]
public sealed class CreateClientRequestHandlerTests
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
        this.validatorMock = new Mock<IValidator<Client>>();
        this.loggerMock = new Mock<ILogger<CreateClientRequestHandler>>();

        this.handler = new CreateClientRequestHandler(
            this.userManagerMock.Object,
            this.unitOfWorkMock.Object,
            this.repositoryClientMock.Object,
            this.tenantProviderMock.Object,
            this.userContextMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region CreateClient Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldCreateClient_Successfully()
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
        user.AssociateTenant(tenantId);

        CreateClientRequest request = new(
            "Ricardo",
            "ricardo@gmail.com",
            "(51) 90000-0001",
            "RS",
            "Carazinho",
            "Marcondes",
            "Edi Marcondes",
            33,
            "000.000.000-01"
        );

        this.userContextMock
            .Setup(t => t.GetUserId())
            .Returns(userId);

        this.userManagerMock
            .Setup(u => u.FindByIdAsync(this.userContextMock.Object.GetUserId().ToString()))
            .ReturnsAsync(user);

        Address address = new(
            request.State,
            request.City,
            request.Neighborhood,
            request.Street,
            request.Number
        );

        Client client = new(
            request.FullName,
            request.Email,
            request.PhoneNumber,
            request.Document,
            address
        );

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<Client>(),
                CancellationToken.None
                ))
            .ReturnsAsync(new ValidationResult());

        this.repositoryClientMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        // Act
        Result<CreateClientResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.IsAny<Client>(),
                CancellationToken.None
                ), Times.Once
            );

        this.repositoryClientMock
            .Verify(r => r.GetAllAsync(), Times.Once);

        this.repositoryClientMock
            .Verify(r => r.AddAsync(
                It.IsAny<Client>()), Times.Once
            );

        this.unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}