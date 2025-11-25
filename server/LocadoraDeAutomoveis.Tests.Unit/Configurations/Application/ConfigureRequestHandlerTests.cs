using LocadoraDeAutomoveis.Application.Configurations.Commands.Configure;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Configurations;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Configurations.Application;

[TestClass]
[TestCategory("Configure Application - Unit Tests")]
public sealed class ConfigureRequestHandlerTests
{
    private ConfigureRequestHandler handler = null!;

    private const string _userName = "Clebinho";
    private const string _fullName = "Clebinho Da Silva";
    private const string _email = "cleber@dasilva.net";
    private const string _phoneNumber = "(99) 99999-9999";

    private Mock<UserManager<User>> userManagerMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryConfiguration> repositoryConfigurationMock = null!;
    private Mock<ITenantProvider> tenantProviderMock = null!;
    private Mock<IUserContext> userContextMock = null!;
    private Mock<IValidator<Configuration>> validatorMock = null!;
    private Mock<ILogger<ConfigureRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!
        );

        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryConfigurationMock = new Mock<IRepositoryConfiguration>();
        this.tenantProviderMock = new Mock<ITenantProvider>();
        this.userContextMock = new Mock<IUserContext>();
        this.validatorMock = new Mock<IValidator<Configuration>>();
        this.loggerMock = new Mock<ILogger<ConfigureRequestHandler>>();

        this.handler = new ConfigureRequestHandler(
            this.userManagerMock.Object,
            this.unitOfWorkMock.Object,
            this.repositoryConfigurationMock.Object,
            this.tenantProviderMock.Object,
            this.userContextMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region CreateGroup Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldConfigure_Successfully()
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
            TenantId = tenantId
        };

        ConfigureRequest request = new(
            25m,
            30m,
            20m,
            15m
        );

        this.userContextMock
            .Setup(t => t.GetUserId())
            .Returns(userId);

        this.userManagerMock
            .Setup(u => u.FindByIdAsync(this.userContextMock.Object.GetUserId().ToString()))
            .ReturnsAsync(user);

        Configuration config = new(
            request.GasolinePrice,
            request.GasPrice,
            request.DieselPrice,
            request.AlcoholPrice
        );

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<Configuration>(c =>
                    c.GasolinePrice == request.GasolinePrice &
                    c.GasPrice == request.GasPrice &&
                    c.DieselPrice == request.DieselPrice &&
                    c.AlcoholPrice == request.AlcoholPrice
                    ), CancellationToken.None
                ))
            .ReturnsAsync(new ValidationResult());

        this.tenantProviderMock
            .Setup(t => t.GetTenantId())
            .Returns(tenantId);

        this.repositoryConfigurationMock
            .Setup(r => r.GetByTenantId(tenantId));

        this.repositoryConfigurationMock
            .Setup(r => r.AddAsync(config))
            .Verifiable();

        // Act
        Result<ConfigureResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.Is<Configuration>(c =>
                    c.GasolinePrice == request.GasolinePrice &
                    c.GasPrice == request.GasPrice &&
                    c.DieselPrice == request.DieselPrice &&
                    c.AlcoholPrice == request.AlcoholPrice
                    ), CancellationToken.None
                ), Times.Once
            );

        this.repositoryConfigurationMock
            .Verify(r => r.GetByTenantId(tenantId), Times.Once);

        this.repositoryConfigurationMock
            .Verify(r => r.AddAsync(
                It.Is<Configuration>(c =>
                    c.GasolinePrice == request.GasolinePrice &
                    c.GasPrice == request.GasPrice &&
                    c.DieselPrice == request.DieselPrice &&
                    c.AlcoholPrice == request.AlcoholPrice
                    )), Times.Once
            );

        Assert.IsTrue(result.IsSuccess);
    }

    [TestMethod]
    public void Handler_ShouldReconfigure_Successfully()
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
            TenantId = tenantId
        };

        ConfigureRequest request = new(
            25m,
            30m,
            20m,
            15m
        );

        this.userContextMock
            .Setup(t => t.GetUserId())
            .Returns(userId);

        this.userManagerMock
            .Setup(u => u.FindByIdAsync(this.userContextMock.Object.GetUserId().ToString()))
            .ReturnsAsync(user);

        Configuration tempConfig = new(
            request.GasolinePrice,
            request.GasPrice,
            request.DieselPrice,
            request.AlcoholPrice
        );

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<Configuration>(c =>
                    c.GasolinePrice == request.GasolinePrice &
                    c.GasPrice == request.GasPrice &&
                    c.DieselPrice == request.DieselPrice &&
                    c.AlcoholPrice == request.AlcoholPrice
                    ), CancellationToken.None
                ))
            .ReturnsAsync(new ValidationResult());

        Configuration config = new(
            request.GasolinePrice,
            request.GasPrice,
            request.DieselPrice,
            request.AlcoholPrice
        );

        this.tenantProviderMock
            .Setup(t => t.GetTenantId())
            .Returns(tenantId);

        this.repositoryConfigurationMock
            .Setup(r => r.GetByTenantId(tenantId))
            .ReturnsAsync(config);

        this.repositoryConfigurationMock
            .Setup(r => r.UpdateAsync(
                config.Id,
                It.Is<Configuration>(c =>
                    c.GasolinePrice == request.GasolinePrice &
                    c.GasPrice == request.GasPrice &&
                    c.DieselPrice == request.DieselPrice &&
                    c.AlcoholPrice == request.AlcoholPrice
                    )))
            .Verifiable();

        // Act
        Result<ConfigureResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.Is<Configuration>(c =>
                    c.GasolinePrice == request.GasolinePrice &
                    c.GasPrice == request.GasPrice &&
                    c.DieselPrice == request.DieselPrice &&
                    c.AlcoholPrice == request.AlcoholPrice
                    ), CancellationToken.None
                ), Times.Once
            );

        this.repositoryConfigurationMock
            .Verify(r => r.GetByTenantId(tenantId), Times.Once);

        this.repositoryConfigurationMock
            .Verify(r => r.UpdateAsync(
                config.Id,
                It.Is<Configuration>(c =>
                    c.GasolinePrice == request.GasolinePrice &
                    c.GasPrice == request.GasPrice &&
                    c.DieselPrice == request.DieselPrice &&
                    c.AlcoholPrice == request.AlcoholPrice
                    )
                ), Times.Once
            );

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
