using LocadoraDeAutomoveis.Application.Coupons.Commands.Delete;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Coupons.Application;

[TestClass]
[TestCategory("Coupon Application - Unit Tests")]
public sealed class DeleteCouponRequestHandlerTest
{
    private DeleteCouponRequestHandler handler = null!;

    private Mock<IUnitOfWork> unitOfWorkMock = null!;
    private Mock<IRepositoryCoupon> repositoryCouponMock = null!;
    private Mock<IRepositoryRental> repositoryRentalMock = null!;
    private Mock<ILogger<DeleteCouponRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.repositoryCouponMock = new Mock<IRepositoryCoupon>();
        this.repositoryRentalMock = new Mock<IRepositoryRental>();
        this.loggerMock = new Mock<ILogger<DeleteCouponRequestHandler>>();

        this.handler = new DeleteCouponRequestHandler(
            this.unitOfWorkMock.Object,
            this.repositoryCouponMock.Object,
            this.repositoryRentalMock.Object,
            this.loggerMock.Object
        );
    }

    #region DeleteCoupon Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldDeleteCoupon_Successfully()
    {
        // Arrange
        Guid couponId = Guid.NewGuid();
        DeleteCouponRequest request = new(
            couponId
        );

        Coupon coupon = new(
            "PROMO25",
            25,
            DateTimeOffset.UtcNow.AddDays(5)
        )
        { Id = couponId };

        this.repositoryCouponMock
            .Setup(r => r.GetByIdAsync(couponId))
            .ReturnsAsync(coupon);

        this.repositoryCouponMock
            .Setup(r => r.DeleteAsync(request.Id))
            .Verifiable();

        this.unitOfWorkMock
            .Setup(u => u.CommitAsync()
            ).Verifiable();

        // Act
        Result<DeleteCouponResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        // Assert
        this.repositoryCouponMock
            .Verify(r => r.GetByIdAsync(couponId),
            Times.Once);

        this.repositoryCouponMock
            .Verify(r => r.DeleteAsync(request.Id), Times.Once);

        this.unitOfWorkMock
            .Verify(u => u.CommitAsync(), Times.Once);

        Assert.IsTrue(result.IsSuccess);
    }
    #endregion
}
