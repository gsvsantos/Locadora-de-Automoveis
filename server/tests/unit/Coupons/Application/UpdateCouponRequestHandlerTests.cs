using LocadoraDeAutomoveis.Application.Coupons.Commands.Update;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Partners;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Tests.Unit.Shared;
using Microsoft.Extensions.Caching.Distributed;

namespace LocadoraDeAutomoveis.Tests.Unit.Coupons.Application;

[TestClass]
[TestCategory("Coupon Application - Unit Tests")]
public sealed class UpdateCouponRequestHandlerTests : UnitTestBase
{
    private UpdateCouponRequestHandler handler = null!;

    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryCoupon> repositoryCouponMock = null!;
    private Mock<IRepositoryPartner> repositoryPartnerMock = null!;
    private Mock<IDistributedCache> cacheMock = null!;
    private Mock<IValidator<Coupon>> validatorMock = null!;
    private Mock<ILogger<UpdateCouponRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryCouponMock = new Mock<IRepositoryCoupon>();
        this.repositoryPartnerMock = new Mock<IRepositoryPartner>();
        this.cacheMock = new Mock<IDistributedCache>();
        this.validatorMock = new Mock<IValidator<Coupon>>();
        this.loggerMock = new Mock<ILogger<UpdateCouponRequestHandler>>();

        this.handler = new UpdateCouponRequestHandler(
            this.unitOfWorkMock.Object,
            this.mapper,
            this.repositoryCouponMock.Object,
            this.repositoryPartnerMock.Object,
            this.cacheMock.Object,
            this.validatorMock.Object,
            this.loggerMock.Object
        );
    }

    #region UpdateCoupon Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldUpdateCoupon_Successfully()
    {
        // Arrange
        Guid partnerId = Guid.NewGuid();
        Partner partner = new(
            "MACHINIMA"
        )
        { Id = partnerId };

        Guid couponId = Guid.NewGuid();
        Coupon coupon = new(
            "PROMO25",
            25,
            DateTimeOffset.UtcNow.AddDays(5)
        )
        { Id = couponId };
        coupon.AssociatePartner(partner);

        this.repositoryCouponMock
            .Setup(r => r.GetByIdAsync(couponId))
            .ReturnsAsync(coupon);

        Guid partner2Id = Guid.NewGuid();
        Partner partner2 = new(
            "G2A"
        )
        { Id = partner2Id };

        UpdateCouponRequest request = new(
            couponId,
            "PROMO50",
            50,
            DateTimeOffset.UtcNow.AddDays(30),
            partner2Id
        );

        this.repositoryPartnerMock
            .Setup(r => r.GetByIdAsync(partner2Id))
            .ReturnsAsync(partner2);

        Coupon updatedCoupon = new(
            request.Name,
            request.DiscountValue,
            request.ExpirationDate
        );
        updatedCoupon.AssociatePartner(partner2);

        this.validatorMock
            .Setup(v => v.ValidateAsync(
                It.IsAny<Coupon>(), CancellationToken.None
                )
            ).ReturnsAsync(new ValidationResult());

        this.repositoryCouponMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync([coupon]);

        this.repositoryCouponMock
            .Setup(r => r.UpdateAsync(request.Id, updatedCoupon))
            .Verifiable();

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync()
            ).Verifiable();

        // Act
        Result<UpdateCouponResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.repositoryCouponMock
            .Verify(r => r.GetByIdAsync(couponId),
            Times.Once);

        this.repositoryPartnerMock
            .Verify(r => r.GetByIdAsync(partner2Id),
            Times.Once);

        this.validatorMock
            .Verify(v => v.ValidateAsync(It.IsAny<Coupon>(), CancellationToken.None
                ), Times.Once
            );

        this.repositoryCouponMock
            .Verify(r => r.GetAllAsync(), Times.Once);

        this.repositoryCouponMock
            .Verify(r => r.UpdateAsync(request.Id, It.IsAny<Coupon>()
                ), Times.Once
            );

        this.unitOfWorkMock
            .Verify(u => u.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(couponId, result.Value.Id);
    }
    #endregion
}
