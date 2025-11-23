using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.PricingPlans;

namespace LocadoraDeAutomoveis.Tests.Integration.Repositories;

[TestClass]
[TestCategory("PricingPlanRepository Infrastructure - Integration Tests")]
public sealed class PricingPlanRepositoryTests : TestFixture
{
    [TestMethod]
    public async Task Should_GetAllAsync_ReturnsPricingPlanWithGroup_Successfully()
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

        List<Group> existingGroups = Builder<Group>.CreateListOfSize(2).Build().ToList();
        foreach (Group group in existingGroups)
        {
            group.AssociateTenant(tenant.Id);
            group.AssociateUser(userEmployee);
        }

        await this.groupRepository.AddMultiplyAsync(existingGroups);

        await this.dbContext.SaveChangesAsync();

        RandomGenerator random = new();
        List<PricingPlan> existingPricingPlans = Builder<PricingPlan>.CreateListOfSize(10).All()
            .Do(v => v.DailyPlan = new(random.Decimal(), random.Decimal()))
            .Do(v => v.ControlledPlan = new(random.Decimal(), random.Decimal(), random.Int()))
            .Do(v => v.FreePlan = new(random.Decimal()))
            .Build().ToList();
        foreach (PricingPlan pricingPlan in existingPricingPlans)
        {
            pricingPlan.AssociateTenant(tenant.Id);
            pricingPlan.AssociateUser(userEmployee);
            pricingPlan.AssociateGroup(existingGroups[0]);
        }

        existingPricingPlans[4].AssociateGroup(existingGroups[1]);
        existingPricingPlans[8].AssociateGroup(existingGroups[1]);

        await this.pricingPlanRepository.AddMultiplyAsync(existingPricingPlans);

        await this.dbContext.SaveChangesAsync();

        // Act 
        List<PricingPlan> pricingPlans = await this.pricingPlanRepository.GetAllAsync();

        // Assert
        CollectionAssert.AreEquivalent(existingPricingPlans, pricingPlans);
        Assert.AreEqual(10, pricingPlans.Count);
        Assert.AreEqual(existingGroups[0], existingPricingPlans[2].Group);
        Assert.AreEqual(existingGroups[1], existingPricingPlans[4].Group);
    }

    [TestMethod]
    public async Task Should_GetFiveAsync_ReturnsPricingPlanWithGroup_Successfully()
    {
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

        List<Group> existingGroups = Builder<Group>.CreateListOfSize(2).Build().ToList();
        foreach (Group group in existingGroups)
        {
            group.AssociateTenant(tenant.Id);
            group.AssociateUser(userEmployee);
        }

        await this.groupRepository.AddMultiplyAsync(existingGroups);

        await this.dbContext.SaveChangesAsync();

        RandomGenerator random = new();
        List<PricingPlan> existingPricingPlans = Builder<PricingPlan>.CreateListOfSize(10).All()
            .Do(v => v.DailyPlan = new(random.Decimal(), random.Decimal()))
            .Do(v => v.ControlledPlan = new(random.Decimal(), random.Decimal(), random.Int()))
            .Do(v => v.FreePlan = new(random.Decimal()))
            .Build().ToList();
        foreach (PricingPlan pricingPlan in existingPricingPlans)
        {
            pricingPlan.AssociateTenant(tenant.Id);
            pricingPlan.AssociateUser(userEmployee);
            pricingPlan.AssociateGroup(existingGroups[0]);
        }

        existingPricingPlans[1].AssociateGroup(existingGroups[1]);
        existingPricingPlans[4].AssociateGroup(existingGroups[1]);

        await this.pricingPlanRepository.AddMultiplyAsync(existingPricingPlans);

        await this.dbContext.SaveChangesAsync();

        // Act 
        List<PricingPlan> pricingPlans = await this.pricingPlanRepository.GetAllAsync(5);

        // Assert
        Assert.AreEqual(5, pricingPlans.Count);
        Assert.AreEqual(existingGroups[1], existingPricingPlans[1].Group);
        Assert.AreEqual(existingGroups[0], existingPricingPlans[2].Group);
        Assert.AreEqual(existingGroups[0], existingPricingPlans[3].Group);
        Assert.AreEqual(existingGroups[1], existingPricingPlans[4].Group);
        Assert.AreEqual(existingGroups[0], existingPricingPlans[5].Group);
    }

    [TestMethod]
    public async Task Should_GetPricingPlanByIdAsync_ReturnsPricingPlanWithGroup_Successfully()
    {
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

        Group group = Builder<Group>.CreateNew().Build();
        group.AssociateTenant(tenant.Id);
        group.AssociateUser(userEmployee);

        RandomGenerator random = new();
        PricingPlan pricingPlan = Builder<PricingPlan>.CreateNew()
            .Do(v => v.DailyPlan = new(random.Decimal(), random.Decimal()))
            .Do(v => v.ControlledPlan = new(random.Decimal(), random.Decimal(), random.Int()))
            .Do(v => v.FreePlan = new(random.Decimal()))
            .Build();

        pricingPlan.AssociateTenant(tenant.Id);
        pricingPlan.AssociateUser(userEmployee);
        pricingPlan.AssociateGroup(group);

        await this.pricingPlanRepository.AddAsync(pricingPlan);

        await this.dbContext.SaveChangesAsync();

        // Act 
        PricingPlan? selectedPricingPlan = await this.pricingPlanRepository.GetByIdAsync(pricingPlan.Id);

        // Assert
        Assert.IsNotNull(selectedPricingPlan);
        Assert.AreEqual(group, selectedPricingPlan.Group);
    }
}
