using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Partners;

namespace LocadoraDeAutomoveis.Tests.Integration.Repositories;

[TestClass]
[TestCategory("CouponRepository Infrastructure - Integration Tests")]
public sealed class CouponRepositoryTests : TestFixture
{
    [TestMethod]
    public async Task Should_GetAllAsync_ReturnCouponsWithPartner_Successfully()
    {
        // Arrange
        User tenant = Builder<User>.CreateNew()
            .With(t => t.FullName = "Tenant User")
            .With(t => t.UserName = $"tenantUser-{Guid.NewGuid()}")
            .With(t => t.Id = Guid.NewGuid()).Persist();
        tenant.AssociateTenant(tenant.Id);

        User userEmployee = Builder<User>.CreateNew()
            .With(uE => uE.FullName = "Employee User")
            .With(uE => uE.UserName = $"employeeUser-{Guid.NewGuid()}")
            .With(uE => uE.Id = Guid.NewGuid()).Persist();
        userEmployee.AssociateTenant(tenant.Id);

        Partner partner = Builder<Partner>.CreateNew()
            .With(p => p.FullName = "Partner User")
            .Build();
        partner.AssociateTenant(tenant.Id);
        partner.AssociateUser(userEmployee);

        await this.dbContext.Partners.AddAsync(partner);

        await this.dbContext.SaveChangesAsync();

        List<Coupon> existingCoupons = Builder<Coupon>.CreateListOfSize(10).All()
            .With(c => c.Name = $"CUPOM-{Guid.NewGuid()}")
            .With(c => c.ExpirationDate = DateTimeOffset.UtcNow.AddDays(10))
            .Build().ToList();

        foreach (Coupon coupon in existingCoupons)
        {
            coupon.AssociateTenant(tenant.Id);
            coupon.AssociateUser(userEmployee);
            coupon.AssociatePartner(partner);
        }

        await this.couponRepository.AddMultiplyAsync(existingCoupons);

        await this.dbContext.SaveChangesAsync();

        // Act
        List<Coupon> coupons = await this.couponRepository.GetAllAsync();

        CollectionAssert.AreEquivalent(existingCoupons, coupons);
        Assert.AreEqual(10, coupons.Count);
        Assert.IsTrue(existingCoupons[3].Partner is not null);
        Assert.AreEqual(partner, existingCoupons[3].Partner);
    }

    [TestMethod]
    public async Task Should_GetFiveAsync_ReturnCouponsWithPartner_Successfully()
    {
        // Arrange
        User tenant = Builder<User>.CreateNew()
            .With(t => t.FullName = "Tenant User")
            .With(t => t.UserName = $"tenantUser-{Guid.NewGuid()}")
            .With(t => t.Id = Guid.NewGuid()).Persist();
        tenant.AssociateTenant(tenant.Id);

        User userEmployee = Builder<User>.CreateNew()
            .With(uE => uE.FullName = "Employee User")
            .With(uE => uE.UserName = $"employeeUser-{Guid.NewGuid()}")
            .With(uE => uE.Id = Guid.NewGuid()).Persist();
        userEmployee.AssociateTenant(tenant.Id);

        Partner partner = Builder<Partner>.CreateNew()
            .With(p => p.FullName = "Partner User")
            .Build();
        partner.AssociateTenant(tenant.Id);
        partner.AssociateUser(userEmployee);

        await this.dbContext.Partners.AddAsync(partner);

        await this.dbContext.SaveChangesAsync();

        List<Coupon> existingCoupons = Builder<Coupon>.CreateListOfSize(10).All()
            .With(c => c.Name = $"CUPOM-{Guid.NewGuid()}")
            .With(c => c.ExpirationDate = DateTimeOffset.UtcNow.AddDays(10))
            .Build().ToList();

        foreach (Coupon coupon in existingCoupons)
        {
            coupon.AssociateTenant(tenant.Id);
            coupon.AssociateUser(userEmployee);
            coupon.AssociatePartner(partner);
        }

        await this.couponRepository.AddMultiplyAsync(existingCoupons);

        await this.dbContext.SaveChangesAsync();

        // Act
        List<Coupon> coupons = await this.couponRepository.GetAllAsync(5);

        // Assert
        Assert.AreEqual(5, coupons.Count);
        Assert.IsTrue(existingCoupons[3].Partner is not null);
        Assert.AreEqual(partner, existingCoupons[3].Partner);

        Coupon loadedCoupon = coupons.First();
        Assert.IsNotNull(loadedCoupon.Partner);
        Assert.AreEqual(partner.Id, loadedCoupon.Partner.Id);
    }

    [TestMethod]
    public async Task Should_GetCouponById_ReturnCouponWithPartner_Successfully()
    {
        // Arrange
        User tenant = Builder<User>.CreateNew()
            .With(t => t.FullName = "Tenant User")
            .With(t => t.UserName = $"tenantUser-{Guid.NewGuid()}")
            .With(t => t.Id = Guid.NewGuid()).Persist();
        tenant.AssociateTenant(tenant.Id);

        User userEmployee = Builder<User>.CreateNew()
            .With(uE => uE.FullName = "Employee User")
            .With(uE => uE.UserName = $"employeeUser-{Guid.NewGuid()}")
            .With(uE => uE.Id = Guid.NewGuid()).Persist();
        userEmployee.AssociateTenant(tenant.Id);

        Partner partner = Builder<Partner>.CreateNew()
            .With(p => p.FullName = "Partner User")
            .Build();
        partner.AssociateTenant(tenant.Id);
        partner.AssociateUser(userEmployee);

        await this.dbContext.Partners.AddAsync(partner);

        await this.dbContext.SaveChangesAsync();

        Coupon coupon = Builder<Coupon>.CreateNew()
            .With(c => c.ExpirationDate = DateTimeOffset.UtcNow.AddDays(10))
            .Build();

        coupon.AssociateTenant(tenant.Id);
        coupon.AssociateUser(userEmployee);
        coupon.AssociatePartner(partner);

        await this.couponRepository.AddAsync(coupon);
        await this.dbContext.SaveChangesAsync();

        // Act
        Coupon? selectedCoupon = await this.couponRepository.GetByIdAsync(coupon.Id);

        // Assert
        Assert.IsNotNull(selectedCoupon);
        Assert.AreEqual(coupon, selectedCoupon);
        Assert.IsNotNull(selectedCoupon!.Partner);
        Assert.AreEqual(partner.Id, selectedCoupon.Partner.Id);
    }
}
