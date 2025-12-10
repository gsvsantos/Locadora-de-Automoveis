using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Partners;

namespace LocadoraDeAutomoveis.Tests.Unit.Coupons.Domain;

[TestClass]
[TestCategory("Coupon Domain - Unit Tests")]
public sealed class CouponTests
{
    [TestMethod]
    public void CouponConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        Coupon coupon = new();

        // Assert
        Assert.AreNotEqual(Guid.Empty, coupon.Id);
        Assert.IsTrue(coupon.IsActive);
        Assert.AreEqual(string.Empty, coupon.Name);
        Assert.AreEqual(0, coupon.DiscountValue);
        Assert.AreEqual(DateTimeOffset.MinValue, coupon.ExpirationDate);
        Assert.AreEqual(Guid.Empty, coupon.PartnerId);
        Assert.IsNull(coupon.Partner);
        Assert.IsFalse(coupon.IsManuallyDisabled);
    }

    [TestMethod]
    public void CouponConstructor_Parameterized_ShouldWorks()
    {
        // Arrange & Act
        DateTimeOffset date = DateTimeOffset.UtcNow.AddDays(5);
        Coupon coupon = new(
            "PROMO25",
            25,
            date
        );

        // Assert
        Assert.AreNotEqual(Guid.Empty, coupon.Id);
        Assert.IsTrue(coupon.IsActive);
        Assert.AreEqual("PROMO25", coupon.Name);
        Assert.AreEqual(25, coupon.DiscountValue);
        Assert.AreEqual(date, coupon.ExpirationDate);
        Assert.AreEqual(Guid.Empty, coupon.PartnerId);
        Assert.IsNull(coupon.Partner);
        Assert.IsFalse(coupon.IsManuallyDisabled);
    }

    [TestMethod]
    public void CouponMethod_Update_ShouldWorks()
    {
        // Arrange
        DateTimeOffset date = DateTimeOffset.UtcNow.AddDays(5);
        DateTimeOffset newDate = DateTimeOffset.UtcNow.AddDays(30);

        Coupon coupon = new(
            "PROMO25",
            25,
            date
        );

        Coupon updatedCoupon = new(
            "PROMO50",
            50,
            newDate
        );

        // Act
        coupon.Update(updatedCoupon);

        // Assert
        Assert.AreEqual("PROMO50", coupon.Name);
        Assert.AreEqual(50, coupon.DiscountValue);
        Assert.AreEqual(newDate, coupon.ExpirationDate);
    }

    [TestMethod]
    public void CouponMethod_SetManuallyDisabledTrue_ShouldWorks()
    {
        // Arrange
        Coupon coupon = new();

        // Act
        coupon.SetManuallyDisabledTrue();

        // Assert
        Assert.IsTrue(coupon.IsManuallyDisabled);
    }

    [TestMethod]
    public void CouponMethod_IsExpired_ShouldWorks()
    {
        // Arrange
        DateTimeOffset date = DateTimeOffset.UtcNow.AddDays(-5);
        Coupon coupon = new(
            "PROMO25",
            25,
            date
        );

        // Act
        bool expired = coupon.IsExpired();

        // Assert
        Assert.IsTrue(expired);
    }

    [TestMethod]
    public void CouponMethod_AssociatePartner_ShouldWorks()
    {
        // Arrange
        Coupon coupon = Builder<Coupon>.CreateNew().Build();
        Partner partner = Builder<Partner>.CreateNew().Build();

        // Act
        coupon.AssociatePartner(partner);

        // Assert
        Assert.AreEqual(partner.Id, coupon.PartnerId);
        Assert.AreEqual(partner, coupon.Partner);
    }

    [TestMethod]
    public void CouponMethod_DisassociatePartner_ShouldWorks()
    {
        // Arrange
        Coupon coupon = Builder<Coupon>.CreateNew().Build();
        Partner partner = Builder<Partner>.CreateNew().Build();

        coupon.AssociatePartner(partner);

        // Act
        coupon.DisassociatePartner();

        // Assert
        Assert.AreNotEqual(partner.Id, coupon.PartnerId);
        Assert.AreNotEqual(partner, coupon.Partner);
    }
}