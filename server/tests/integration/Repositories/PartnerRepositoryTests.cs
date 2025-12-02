using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Partners;

namespace LocadoraDeAutomoveis.Tests.Integration.Repositories;

[TestClass]
[TestCategory("PartnerRepository Infrastructure - Integration Tests")]
public sealed class PartnerRepositoryTests : TestFixture
{
    [TestMethod]
    public async Task Should_GetAllAsync_ReturnPartnersWithCoupons_Successfully()
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

        List<Partner> existingPartners = Builder<Partner>.CreateListOfSize(10).Build().ToList();
        foreach (Partner partner in existingPartners)
        {
            partner.AssociateTenant(tenant.Id);
            partner.AssociateUser(userEmployee);
        }

        Coupon coupon = Builder<Coupon>.CreateNew().Build();
        coupon.AssociateTenant(tenant.Id);
        coupon.AssociateUser(userEmployee);

        existingPartners[3].AddCoupon(coupon);

        await this.partnerRepository.AddMultiplyAsync(existingPartners);

        await this.dbContext.SaveChangesAsync();

        // Act
        List<Partner> partners = await this.partnerRepository.GetAllAsync();

        // Assert
        CollectionAssert.AreEquivalent(existingPartners, partners);
        Assert.AreEqual(10, partners.Count);
        Assert.IsTrue(existingPartners[3].Coupons.Count > 0);
        Assert.AreEqual(coupon, existingPartners[3].Coupons[0]);
    }

    [TestMethod]
    public async Task Should_GetFiveAsync_ReturnPartnersWithCoupons_Successfully()
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

        List<Partner> existingPartners = Builder<Partner>.CreateListOfSize(10).Build().ToList();
        foreach (Partner partner in existingPartners)
        {
            partner.AssociateTenant(tenant.Id);
            partner.AssociateUser(userEmployee);
        }

        Coupon coupon = Builder<Coupon>.CreateNew().Build();
        coupon.AssociateTenant(tenant.Id);
        coupon.AssociateUser(userEmployee);

        existingPartners[3].AddCoupon(coupon);

        await this.partnerRepository.AddMultiplyAsync(existingPartners);

        await this.dbContext.SaveChangesAsync();

        // Act
        List<Partner> partners = await this.partnerRepository.GetAllAsync(5);

        // Assert
        Assert.AreEqual(5, partners.Count);
        Assert.IsTrue(existingPartners[3].Coupons.Count > 0);
        Assert.AreEqual(coupon, existingPartners[3].Coupons[0]);
    }

    [TestMethod]
    public async Task Should_GetPartnerById_ReturnPartnersWithCoupons_Successfully()
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

        Partner partner = Builder<Partner>.CreateNew().Build();
        partner.AssociateTenant(tenant.Id);
        partner.AssociateUser(userEmployee);

        Coupon coupon = Builder<Coupon>.CreateNew().Build();
        coupon.AssociateTenant(tenant.Id);
        coupon.AssociateUser(userEmployee);

        partner.AddCoupon(coupon);

        await this.partnerRepository.AddAsync(partner);
        await this.dbContext.SaveChangesAsync();

        // Act
        Partner? selectedPartner = await this.partnerRepository.GetByIdAsync(partner.Id);

        // Assert
        Assert.IsNotNull(selectedPartner);
        Assert.AreEqual(partner, selectedPartner);
        Assert.IsTrue(selectedPartner!.Coupons.Count > 0);
        Assert.AreEqual(coupon, selectedPartner.Coupons[0]);
    }
}
