using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Application.Partners.Commands.GetCoupons;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Partners;

namespace LocadoraDeAutomoveis.Tests.Unit.Partners.Application;

[TestClass]
[TestCategory("Partner Application - Unit Tests")]
public sealed class GetCouponsPartnerRequestHandlerTests
{
    private GetCouponsPartnerRequestHandler handler = null!;

    private Mock<IRepositoryPartner> repositoryPartnerMock = null!;
    private Mock<ILogger<GetCouponsPartnerRequestHandler>> loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        this.repositoryPartnerMock = new Mock<IRepositoryPartner>();
        this.loggerMock = new Mock<ILogger<GetCouponsPartnerRequestHandler>>();

        this.handler = new GetCouponsPartnerRequestHandler(
            this.repositoryPartnerMock.Object,
            this.loggerMock.Object
        );
    }

    #region GetPartnerWithCoupons Tests (Happy Path)
    [TestMethod]
    public void Handler_ShouldGetPartnerWithCoupons_Successfuly()
    {
        // Arrange
        Guid partnerId = Guid.NewGuid();
        GetCouponsPartnerRequest request = new(partnerId);

        Partner partner = new(
            "G2A"
        )
        { Id = partnerId };

        List<Coupon> coupons = Builder<Coupon>.CreateListOfSize(3).All()
            .Do(c => c.AssociatePartner(partner))
            .Build().ToList();

        partner.AddRangeCoupons(coupons);

        this.repositoryPartnerMock
            .Setup(repo => repo.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);

        // Act
        Result<GetCouponsPartnerResponse> result = this.handler.Handle(request, CancellationToken.None).Result;

        GetCouponsPartnerDto dto = result.Value.Partner;

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(partner.Id, dto.Id);
        Assert.AreEqual(partner.FullName, dto.FullName);
        Assert.AreEqual(3, partner.Coupons.Count);
        foreach (Coupon couponEntity in coupons)
        {
            CouponDto? couponDto = dto.Coupons.FirstOrDefault(c => c.Id == couponEntity.Id);

            Assert.IsNotNull(couponDto);
            Assert.AreEqual(couponEntity.Name, couponDto.Name);
            Assert.AreEqual(couponEntity.DiscountValue, couponDto.DiscountValue);
            Assert.AreEqual(couponEntity.ExpirationDate, couponDto.ExpirationDate);
        }
    }
    #endregion
}
