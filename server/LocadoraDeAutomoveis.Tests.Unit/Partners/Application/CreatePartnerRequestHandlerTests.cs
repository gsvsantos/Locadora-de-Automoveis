using LocadoraDeAutomoveis.Application.Partners.Commands.Create;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Partners;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Partners.Application;

[TestClass]
[TestCategory("Partner Application - Unit Tests")]
public sealed class CreatePartnerRequestHandlerTests
{
    private CreatePartnerRequestHandler handler = null!;

    private const string _userName = "Clebinho";
    private const string _fullName = "Clebinho Da Silva";
    private const string _email = "cleber@dasilva.net";
    private const string _phoneNumber = "(99) 99999-9999";

    private Mock<UserManager<User>> userManagerMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryPartner> repositoryPartnerMock = null!;
    private Mock<ITenantProvider> tenantProviderMock = null!;
    private Mock<IUserContext> userContextMock = null!;
    private Mock<IValidator<Partner>> validatorMock = null!;
    private Mock<ILogger<CreatePartnerRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!
        );

        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryPartnerMock = new Mock<IRepositoryPartner>();
        this.tenantProviderMock = new Mock<ITenantProvider>();
        this.userContextMock = new Mock<IUserContext>();
        this.validatorMock = new Mock<IValidator<Partner>>();
        this.loggerMock = new Mock<ILogger<CreatePartnerRequestHandler>>();

        this.handler = new CreatePartnerRequestHandler(
            this.userManagerMock.Object,
            this.unitOfWorkMock.Object,
            this.repositoryPartnerMock.Object,
            this.tenantProviderMock.Object,
            this.userContextMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region CreatePartner Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldCreatePartner_Successfully()
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

        CreatePartnerRequest request = new("G2A");

        this.userContextMock
            .Setup(t => t.GetUserId())
            .Returns(userId);

        this.userManagerMock
            .Setup(u => u.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        Partner partner = new(
            request.FullName
        );

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<Partner>(p =>
                    p.FullName == request.FullName
                    ), CancellationToken.None
                )
            ).ReturnsAsync(new ValidationResult());

        this.repositoryPartnerMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        this.repositoryPartnerMock
            .Setup(r => r.AddAsync(
                It.Is<Partner>(p =>
                    p.FullName == request.FullName
                ))
            ).Verifiable();

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync()
            ).Verifiable();

        // Act
        Result<CreatePartnerResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.Is<Partner>(p =>
                    p.FullName == request.FullName
                    ), CancellationToken.None
                ), Times.Once
            );

        this.repositoryPartnerMock
            .Verify(r => r.GetAllAsync(), Times.Once);

        this.repositoryPartnerMock
            .Verify(r => r.AddAsync(
                It.Is<Partner>(p =>
                    p.FullName == request.FullName
                    )), Times.Once
            );

        this.unitOfWorkMock
            .Verify(u => u.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
