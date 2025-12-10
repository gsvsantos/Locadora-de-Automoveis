using LocadoraDeAutomoveis.Application.Coupons.Commands.Create;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Partners;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Tests.Unit.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Coupons.Application;

[TestClass]
[TestCategory("Coupon Application - Unit Tests")]
public sealed class CreateCouponRequestHandlerTests : UnitTestBase
{
    private CreateCouponRequestHandler handler = null!;

    private const string _userName = "Clebinho";
    private const string _fullName = "Clebinho Da Silva";
    private const string _email = "cleber@dasilva.net";
    private const string _phoneNumber = "(99) 99999-9999";

    private Mock<UserManager<User>> userManagerMock = null!;
    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryCoupon> repositoryCouponMock = null!;
    private Mock<IRepositoryPartner> repositoryPartnerMock = null!;
    private Mock<ITenantProvider> tenantProviderMock = null!;
    private Mock<IUserContext> userContextMock = null!;
    private Mock<IValidator<Coupon>> validatorMock = null!;
    private Mock<ILogger<CreateCouponRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!
        );

        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryCouponMock = new Mock<IRepositoryCoupon>();
        this.repositoryPartnerMock = new Mock<IRepositoryPartner>();
        this.tenantProviderMock = new Mock<ITenantProvider>();
        this.userContextMock = new Mock<IUserContext>();
        this.validatorMock = new Mock<IValidator<Coupon>>();
        this.loggerMock = new Mock<ILogger<CreateCouponRequestHandler>>();

        this.handler = new CreateCouponRequestHandler(
            this.userManagerMock.Object,
            this.unitOfWorkMock.Object,
            this.mapper,
            this.repositoryCouponMock.Object,
            this.repositoryPartnerMock.Object,
            this.tenantProviderMock.Object,
            this.userContextMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region CreateCoupon Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldCreateCoupon_Successfully()
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

        Guid partnerId = Guid.NewGuid();
        Partner partner = new(
            "G2A"
        )
        { Id = partnerId };

        this.repositoryPartnerMock
            .Setup(r => r.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);

        CreateCouponRequest request = new(
            "G2A",
            50,
            DateTimeOffset.UtcNow.AddDays(30),
            partnerId
        );

        this.userContextMock
            .Setup(t => t.GetUserId())
            .Returns(userId);

        this.userManagerMock
            .Setup(u => u.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        Coupon coupon = new(
            request.Name,
            request.DiscountValue,
            request.ExpirationDate
        );
        coupon.AssociatePartner(partner);

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<Coupon>(c =>
                    c.Name == request.Name &&
                    c.DiscountValue == request.DiscountValue &&
                    c.ExpirationDate == request.ExpirationDate &&
                    c.Partner.Id == request.PartnerId
                    ), CancellationToken.None
                )
            ).ReturnsAsync(new ValidationResult());

        this.repositoryCouponMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([]);

        this.repositoryCouponMock
            .Setup(r => r.AddAsync(
                It.Is<Coupon>(c =>
                    c.Name == request.Name &&
                    c.DiscountValue == request.DiscountValue &&
                    c.ExpirationDate == request.ExpirationDate &&
                    c.Partner.Id == request.PartnerId
                    ))
            ).Verifiable();

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync()
            ).Verifiable();

        // Act
        Result<CreateCouponResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.repositoryPartnerMock
            .Verify(r => r.GetByIdAsync(partnerId),
            Times.Once);

        this.validatorMock
            .Verify(v => v.ValidateAsync(
                It.Is<Coupon>(c =>
                    c.Name == request.Name &&
                    c.DiscountValue == request.DiscountValue &&
                    c.ExpirationDate == request.ExpirationDate &&
                    c.Partner.Id == request.PartnerId
                    ), CancellationToken.None
                ), Times.Once
            );

        this.repositoryCouponMock
            .Verify(r => r.GetAllAsync(), Times.Once);

        this.repositoryCouponMock
            .Verify(r => r.AddAsync(
                It.Is<Coupon>(c =>
                    c.Name == request.Name &&
                    c.DiscountValue == request.DiscountValue &&
                    c.ExpirationDate == request.ExpirationDate &&
                    c.Partner.Id == request.PartnerId
                    )), Times.Once
            );

        this.unitOfWorkMock
            .Verify(u => u.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
