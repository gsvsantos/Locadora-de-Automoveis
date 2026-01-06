using LocadoraDeAutomoveis.Application.RentalExtras.Commands.Create;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Tests.Unit.Shared;
using Microsoft.Extensions.Caching.Distributed;

namespace LocadoraDeAutomoveis.Tests.Unit.RentalExtras.Application;

[TestClass]
[TestCategory("RentalExtra Application - Unit Tests")]
public sealed class CreateRentalExtraRequestHandlerTests : UnitTestBase
{
    private CreateRentalExtraRequestHandler handler = null!;

    private const string _userName = "Clebinho";
    private const string _fullName = "Clebinho Da Silva";
    private const string _email = "cleber@dasilva.net";
    private const string _phoneNumber = "(99) 99999-9999";

    private Mock<UserManager<User>> userManagerMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryRentalExtra> repositoryRentalExtraMock = null!;
    private Mock<IDistributedCache> cacheMock = null!;
    private Mock<ITenantProvider> tenantProviderMock = null!;
    private Mock<IUserContext> userContextMock = null!;
    private Mock<IValidator<RentalExtra>> validatorMock = null!;
    private Mock<ILogger<CreateRentalExtraRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!
        );

        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryRentalExtraMock = new Mock<IRepositoryRentalExtra>();
        this.cacheMock = new Mock<IDistributedCache>();
        this.tenantProviderMock = new Mock<ITenantProvider>();
        this.userContextMock = new Mock<IUserContext>();
        this.validatorMock = new Mock<IValidator<RentalExtra>>();
        this.loggerMock = new Mock<ILogger<CreateRentalExtraRequestHandler>>();

        this.handler = new CreateRentalExtraRequestHandler(
            this.userManagerMock.Object,
            this.unitOfWorkMock.Object,
            this.mapper,
            this.repositoryRentalExtraMock.Object,
            this.cacheMock.Object,
            this.tenantProviderMock.Object,
            this.userContextMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region CreateRentalExtra Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldCreateRentalExtra_Successfully()
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

        CreateRentalExtraRequest request = new(
            "GPS",
            20,
            true,
            EExtraType.Equipment
        );

        this.userContextMock
            .Setup(t => t.GetUserId())
            .Returns(userId);

        this.userManagerMock
            .Setup(u => u.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        RentalExtra rentalExtra = new(
            request.Name,
            request.Price
        );
        rentalExtra.MarkAsFixed();

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<RentalExtra>(rS =>
                    rS.Name == request.Name &&
                    rS.Price == request.Price
                    ), CancellationToken.None
                ))
            .ReturnsAsync(new ValidationResult());

        this.repositoryRentalExtraMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        // Act
        Result<CreateRentalExtraResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.Is<RentalExtra>(rS =>
                rS.Name == request.Name &&
                rS.Price == request.Price
                    ), CancellationToken.None
                ), Times.Once
            );

        this.repositoryRentalExtraMock
            .Verify(r => r.GetAllAsync(), Times.Once);

        this.repositoryRentalExtraMock
            .Verify(r => r.AddAsync(
                It.Is<RentalExtra>(rS =>
                rS.Name == request.Name &&
                rS.Price == request.Price &&
                rS.IsDaily == request.IsDaily
                )), Times.Once
            );

        this.unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}