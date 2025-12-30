using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Partners;

namespace LocadoraDeAutomoveis.Tests.Unit.Partners.Domain;

[TestClass]
[TestCategory("Partner Domain - Unit Tests")]
public sealed class PartnerTests
{
    [TestMethod]
    public void PartnerConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        Partner partner = new();

        // Assert
        Assert.AreNotEqual(Guid.Empty, partner.Id);
        Assert.IsTrue(partner.IsActive);
        Assert.AreEqual(string.Empty, partner.FullName);
        Assert.AreEqual(0, partner.Coupons.Count);
    }

    [TestMethod]
    public void PartnerConstructor_Parameterized_ShouldWorks()
    {
        // Arrange & Act
        Partner partner = new("G2A");

        // Assert
        Assert.AreNotEqual(Guid.Empty, partner.Id);
        Assert.IsTrue(partner.IsActive);
        Assert.AreEqual("G2A", partner.FullName);
        Assert.AreEqual(0, partner.Coupons.Count);
    }

    [TestMethod]
    public void PartnerMethod_Update_ShouldWorks()
    {
        // Arrange
        Partner partner = new("MACHINIMA");
        Partner updatedPartner = new("G2A");

        // Act
        partner.Update(updatedPartner);

        // Assert
        Assert.AreEqual("G2A", partner.FullName);
    }

    [TestMethod]
    public void PartnerMethod_AddCoupon_ShouldWorks()
    {
        // Arrange
        Partner partner = new("G2A");
        Coupon coupon = Builder<Coupon>.CreateNew()
            .Do(c => c.AssociatePartner(partner))
            .Build();

        // Act
        partner.AddCoupon(coupon);

        // Assert
        Assert.AreEqual(1, partner.Coupons.Count);
        Assert.AreEqual(coupon, partner.Coupons[0]);
    }

    [TestMethod]
    public void PartnerMethod_RemoveCoupon_ShouldWorks()
    {
        // Arrange
        Partner partner = new("G2A");
        Coupon coupon = Builder<Coupon>.CreateNew()
            .Do(c => c.AssociatePartner(partner))
            .Build();

        partner.AddCoupon(coupon);

        // Act
        partner.RemoveCoupon(coupon);

        // Assert
        Assert.AreEqual(0, partner.Coupons.Count);
    }

    [TestMethod]
    public void PartnerMethod_AddRangeCoupons_ShouldNotAddRangeWhen_PartnerCouponsHasSameValue()
    {
        // Arrange
        Partner partner = new("G2A");
        List<Coupon> coupons = Builder<Coupon>.CreateListOfSize(3).All()
            .Do(c => c.AssociatePartner(partner))
            .Build().ToList();

        // Act
        partner.AddRangeCoupons(coupons);

        // Assert
        Assert.AreEqual(3, partner.Coupons.Count);

        for (int i = 0; i < partner.Coupons.Count; i++)
        {
            Coupon coupon = partner.Coupons[i];

            Assert.IsNotNull(coupon);
            Assert.AreEqual(coupon.Name, coupons[i].Name);
            Assert.AreEqual(coupon.DiscountValue, coupons[i].DiscountValue);
            Assert.AreEqual(coupon.ExpirationDate, coupons[i].ExpirationDate);
        }
    }

    [TestMethod]
    public void PartnerMethod_AddRangeCoupons_ShouldAddRangeWhen_PartnerCouponsIsEmpty()
    {
        // Arrange
        Partner partner = new("MACHINIMA");
        List<Coupon> coupons = Builder<Coupon>.CreateListOfSize(3).All()
            .Build().ToList();

        // Act
        partner.AddRangeCoupons(coupons);

        // Assert
        Assert.AreEqual(3, partner.Coupons.Count);

        for (int i = 0; i < partner.Coupons.Count; i++)
        {
            Coupon coupon = partner.Coupons[i];

            Assert.IsNotNull(coupon);
            Assert.AreEqual(coupon.Name, coupons[i].Name);
            Assert.AreEqual(coupon.DiscountValue, coupons[i].DiscountValue);
            Assert.AreEqual(coupon.ExpirationDate, coupons[i].ExpirationDate);
        }
    }
}
