using LocadoraDeAutomoveis.Application.Drivers.Commands.Create;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Drivers.Application;

[TestClass]
[TestCategory("Driver Application - Unit Tests")]
public sealed class CreateDriverRequestHandlerTests
{
    private CreateDriverRequestHandler handler = null!;

    private const string _userName = "Clebinho";
    private const string _fullName = "Clebinho Da Silva";
    private const string _email = "cleber@dasilva.net";
    private const string _phoneNumber = "(99) 99999-9999";

    private Mock<UserManager<User>> userManagerMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryDriver> repositoryDriverMock = null!;
    private Mock<IRepositoryClient> repositoryClientMock = null!;
    private Mock<ITenantProvider> tenantProviderMock = null!;
    private Mock<IUserContext> userContextMock = null!;
    private Mock<IValidator<Driver>> validatorMock = null!;
    private Mock<ILogger<CreateDriverRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!
        );

        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryDriverMock = new Mock<IRepositoryDriver>();
        this.repositoryClientMock = new Mock<IRepositoryClient>();
        this.tenantProviderMock = new Mock<ITenantProvider>();
        this.userContextMock = new Mock<IUserContext>();
        this.validatorMock = new Mock<IValidator<Driver>>();
        this.loggerMock = new Mock<ILogger<CreateDriverRequestHandler>>();

        this.handler = new CreateDriverRequestHandler(
            this.userManagerMock.Object,
            this.unitOfWorkMock.Object,
            this.repositoryDriverMock.Object,
            this.repositoryClientMock.Object,
            this.tenantProviderMock.Object,
            this.userContextMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region CreateDriver Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldCreateDriver_WhenSelectedPhysicalClient()
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

        this.userContextMock
            .Setup(t => t.GetUserId())
            .Returns(userId);

        this.userManagerMock
            .Setup(u => u.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        Guid clientId = Guid.NewGuid();
        Client physicalClient = new(
            "João Físico",
            "joao@email.com",
            "fone",
            "000.000.000-01",
            new(
            "RS",
            "Carazinho",
            "Marcondes",
            "Edi Marcondes",
            33)
        )
        { Id = clientId };

        this.repositoryClientMock
            .Setup(r => r.GetByIdAsync(clientId))
            .ReturnsAsync(physicalClient);

        CreateDriverRequest request = new(
            clientId,
            "João Motorista",
            "joao.mot@email.com",
            "(51) 90009-9999",
            "111.111.111-11",
            "12345",
            DateTimeOffset.Now
        );

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<Driver>(d =>
                    d.FullName == request.FullName && d.Email == request.Email &&
                    d.PhoneNumber == request.PhoneNumber && d.Document == request.Document &&
                    d.LicenseNumber == request.LicenseNumber && d.LicenseValidity == request.LicenseValidity
                    ), CancellationToken.None
                ))
            .ReturnsAsync(new ValidationResult());

        Guid driverId = Guid.NewGuid();
        Driver driver = new(
            request.FullName,
            request.Email,
            request.PhoneNumber,
            request.Document,
            request.LicenseNumber,
            request.LicenseValidity
        )
        { Id = driverId };

        this.repositoryClientMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([physicalClient]);

        this.repositoryDriverMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        this.repositoryDriverMock
            .Setup(r => r.AddAsync(
                It.Is<Driver>(d =>
                    d.FullName == request.FullName && d.Email == request.Email &&
                    d.PhoneNumber == request.PhoneNumber && d.Document == request.Document &&
                    d.LicenseNumber == request.LicenseNumber && d.LicenseValidity == request.LicenseValidity
                    ))
            ).Verifiable();

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync()
            ).Verifiable();

        // Act
        Result<CreateDriverResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.userContextMock
            .Verify(u => u.GetUserId(), Times.Once);

        this.userManagerMock
            .Verify(r => r.FindByIdAsync(userId.ToString()),
            Times.Once);

        this.repositoryClientMock
            .Verify(r => r.GetByIdAsync(clientId), Times.Once);

        this.validatorMock
            .Verify(v => v.ValidateAsync(
                    It.Is<Driver>(d =>
                        d.FullName == request.FullName && d.Email == request.Email &&
                        d.PhoneNumber == request.PhoneNumber && d.Document == request.Document &&
                        d.LicenseNumber == request.LicenseNumber && d.LicenseValidity == request.LicenseValidity
                    ), CancellationToken.None
                ), Times.Once
            );

        this.repositoryClientMock
            .Verify(r => r.GetAllAsync(), Times.Once);

        this.repositoryDriverMock
            .Verify(r => r.GetAllAsync(), Times.Once);

        this.repositoryDriverMock
            .Verify(r => r.AddAsync(
                    It.Is<Driver>(d =>
                        d.FullName == request.FullName && d.Email == request.Email &&
                        d.PhoneNumber == request.PhoneNumber && d.Document == request.Document &&
                        d.LicenseNumber == request.LicenseNumber && d.LicenseValidity == request.LicenseValidity)
                ), Times.Once
            );

        this.repositoryClientMock
            .Verify(r => r.AddAsync(It.IsAny<Client>()), Times.Never);

        this.unitOfWorkMock
            .Verify(u => u.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }

    [TestMethod]
    public void Handler_ShouldCreateDriver_And_CreateNewPhysicalClient_WhenSelectedJuridicalClient()
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

        this.userContextMock
            .Setup(t => t.GetUserId())
            .Returns(userId);

        this.userManagerMock
            .Setup(u => u.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        Guid clientId = Guid.NewGuid();
        Client juridicalClient = new(
            "Empresa LTDA",
            "empresa@email.com",
            "(51) 90000-0001",
            "00.000.000/0001-00",
            new(
            "RS",
            "Carazinho",
            "Marcondes",
            "Edi Marcondes",
            33)
        )
        { Id = clientId };
        juridicalClient.MarkAsJuridical();

        this.repositoryClientMock
            .Setup(r => r.GetByIdAsync(clientId))
            .ReturnsAsync(juridicalClient);

        CreateDriverRequest request = new(
            clientId,
            "Cliente Novo",
            "clienteNovo@email.com",
            "(51) 99999-9999",
            "222.222.222-22",
            "12345",
            DateTimeOffset.Now
        );

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<Driver>(d =>
                    d.FullName == request.FullName && d.Email == request.Email &&
                    d.PhoneNumber == request.PhoneNumber && d.Document == request.Document &&
                    d.LicenseNumber == request.LicenseNumber && d.LicenseValidity == request.LicenseValidity
                    ), CancellationToken.None
                ))
            .ReturnsAsync(new ValidationResult());

        this.repositoryClientMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([juridicalClient]);

        this.repositoryDriverMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        this.repositoryClientMock
            .Setup(r => r.AddAsync(
                It.Is<Client>(c =>
                    c.FullName == request.FullName && c.Email == request.Email &&
                    c.PhoneNumber == request.PhoneNumber && c.Document == request.Document &&
                    c.Address == juridicalClient.Address
                    ))
            ).Verifiable();

        this.repositoryDriverMock
            .Setup(r => r.AddAsync(
                It.Is<Driver>(d =>
                    d.FullName == request.FullName && d.Email == request.Email &&
                    d.PhoneNumber == request.PhoneNumber && d.Document == request.Document &&
                    d.LicenseNumber == request.LicenseNumber && d.LicenseValidity == request.LicenseValidity
                    ))
            ).Verifiable();

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync()
            ).Verifiable();

        // Act
        Result<CreateDriverResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.userContextMock
            .Verify(u => u.GetUserId(), Times.Once);

        this.userManagerMock
            .Verify(r => r.FindByIdAsync(userId.ToString()),
            Times.Once);

        this.repositoryClientMock
            .Verify(r => r.GetByIdAsync(clientId), Times.Once);

        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.Is<Driver>(d =>
                    d.FullName == request.FullName && d.Email == request.Email &&
                    d.PhoneNumber == request.PhoneNumber && d.Document == request.Document &&
                    d.LicenseNumber == request.LicenseNumber && d.LicenseValidity == request.LicenseValidity
                    ), CancellationToken.None
                ), Times.Once
            );

        this.repositoryClientMock
            .Verify(r => r.GetAllAsync(), Times.Once);

        this.repositoryDriverMock
            .Verify(r => r.GetAllAsync(), Times.Once);

        this.repositoryClientMock
            .Verify(r => r.AddAsync(
                It.Is<Client>(c =>
                    c.FullName == request.FullName && c.Email == request.Email &&
                    c.PhoneNumber == request.PhoneNumber && c.Document == request.Document &&
                    c.Address == juridicalClient.Address
                    )
                ), Times.Once);

        this.repositoryDriverMock
            .Verify(r => r.AddAsync(
                It.Is<Driver>(d =>
                    d.FullName == request.FullName && d.Email == request.Email &&
                    d.PhoneNumber == request.PhoneNumber && d.Document == request.Document &&
                    d.LicenseNumber == request.LicenseNumber && d.LicenseValidity == request.LicenseValidity
                    )
                ), Times.Once
            );

        this.unitOfWorkMock
            .Verify(u => u.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
