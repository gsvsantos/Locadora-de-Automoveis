using LocadoraDeAutomoveis.Application.RateServices.Commands.Create;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.RateServices;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.RateServices;

[TestClass]
[TestCategory("RateService Application - Unit Tests")]
public sealed class CreateRateServiceRequestHandlerTests
{
    private CreateRateServiceRequestHandler handler = null!;

    private const string _userName = "Clebinho";
    private const string _fullName = "Clebinho Da Silva";
    private const string _email = "cleber@dasilva.net";
    private const string _phoneNumber = "(99) 99999-9999";

    private Mock<UserManager<User>> userManagerMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryRateService> repositoryRateServiceMock = null!;
    private Mock<ITenantProvider> tenantProviderMock = null!;
    private Mock<IUserContext> userContextMock = null!;
    private Mock<IValidator<RateService>> validatorMock = null!;
    private Mock<ILogger<CreateRateServiceRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!
        );

        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryRateServiceMock = new Mock<IRepositoryRateService>();
        this.tenantProviderMock = new Mock<ITenantProvider>();
        this.userContextMock = new Mock<IUserContext>();
        this.validatorMock = new Mock<IValidator<RateService>>();
        this.loggerMock = new Mock<ILogger<CreateRateServiceRequestHandler>>();

        this.handler = new CreateRateServiceRequestHandler(
            this.userManagerMock.Object,
            this.unitOfWorkMock.Object,
            this.repositoryRateServiceMock.Object,
            this.tenantProviderMock.Object,
            this.userContextMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region CreateRateService Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldCreateRateService_Successfully()
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

        CreateRateServiceRequest request = new(
            "GPS",
            20,
            true
        );

        this.userContextMock
            .Setup(t => t.GetUserId())
            .Returns(userId);

        this.userManagerMock
            .Setup(u => u.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        RateService rateService = new(
            request.Name,
            request.Price
        );
        rateService.MarkAsFixed();

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<RateService>(rS =>
                    rS.Name == request.Name &&
                    rS.Price == request.Price
                    ), CancellationToken.None
                ))
            .ReturnsAsync(new ValidationResult());

        this.repositoryRateServiceMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        // Act
        Result<CreateRateServiceResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.Is<RateService>(rS =>
                rS.Name == request.Name &&
                rS.Price == request.Price
                    ), CancellationToken.None
                ), Times.Once
            );

        this.repositoryRateServiceMock
            .Verify(r => r.GetAllAsync(), Times.Once);

        this.repositoryRateServiceMock
            .Verify(r => r.AddAsync(
                It.Is<RateService>(rS =>
                rS.Name == request.Name &&
                rS.Price == request.Price &&
                rS.IsFixed == request.IsFixed
                )), Times.Once
            );

        this.unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}