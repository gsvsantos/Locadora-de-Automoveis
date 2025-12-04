using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Groups;

namespace LocadoraDeAutomoveis.Tests.Integration.Repositories;

[TestClass]
[TestCategory("BillingPlanRepository Infrastructure - Integration Tests")]
public sealed class BillingPlanRepositoryTests : TestFixture
{
    [TestMethod]
    public async Task Should_GetAllAsync_ReturnsBillingPlanWithGroup_Successfully()
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
        List<BillingPlan> existingBillingPlans = Builder<BillingPlan>.CreateListOfSize(10).All()
            .Do(v => v.Daily = new(random.Decimal(), random.Decimal()))
            .Do(v => v.Controlled = new(random.Decimal(), random.Int()))
            .Do(v => v.Free = new(random.Decimal()))
            .Build().ToList();
        foreach (BillingPlan billingPlan in existingBillingPlans)
        {
            billingPlan.AssociateTenant(tenant.Id);
            billingPlan.AssociateUser(userEmployee);
            billingPlan.AssociateGroup(existingGroups[0]);
        }

        existingBillingPlans[4].AssociateGroup(existingGroups[1]);
        existingBillingPlans[8].AssociateGroup(existingGroups[1]);

        await this.BillingPlanRepository.AddMultiplyAsync(existingBillingPlans);

        await this.dbContext.SaveChangesAsync();

        // Act 
        List<BillingPlan> BillingPlans = await this.BillingPlanRepository.GetAllAsync();

        // Assert
        CollectionAssert.AreEquivalent(existingBillingPlans, BillingPlans);
        Assert.AreEqual(10, BillingPlans.Count);
        Assert.AreEqual(existingGroups[0], existingBillingPlans[2].Group);
        Assert.AreEqual(existingGroups[1], existingBillingPlans[4].Group);
    }

    [TestMethod]
    public async Task Should_GetFiveAsync_ReturnsBillingPlanWithGroup_Successfully()
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
        List<BillingPlan> existingBillingPlans = Builder<BillingPlan>.CreateListOfSize(10).All()
            .Do(v => v.Daily = new(random.Decimal(), random.Decimal()))
            .Do(v => v.Controlled = new(random.Decimal(), random.Int()))
            .Do(v => v.Free = new(random.Decimal()))
            .Build().ToList();
        foreach (BillingPlan BillingPlan in existingBillingPlans)
        {
            BillingPlan.AssociateTenant(tenant.Id);
            BillingPlan.AssociateUser(userEmployee);
            BillingPlan.AssociateGroup(existingGroups[0]);
        }

        existingBillingPlans[1].AssociateGroup(existingGroups[1]);
        existingBillingPlans[4].AssociateGroup(existingGroups[1]);

        await this.BillingPlanRepository.AddMultiplyAsync(existingBillingPlans);

        await this.dbContext.SaveChangesAsync();

        // Act 
        List<BillingPlan> BillingPlans = await this.BillingPlanRepository.GetAllAsync(5);

        // Assert
        Assert.AreEqual(5, BillingPlans.Count);
        Assert.AreEqual(existingGroups[1], existingBillingPlans[1].Group);
        Assert.AreEqual(existingGroups[0], existingBillingPlans[2].Group);
        Assert.AreEqual(existingGroups[0], existingBillingPlans[3].Group);
        Assert.AreEqual(existingGroups[1], existingBillingPlans[4].Group);
        Assert.AreEqual(existingGroups[0], existingBillingPlans[5].Group);
    }

    [TestMethod]
    public async Task Should_GetBillingPlanByIdAsync_ReturnsBillingPlanWithGroup_Successfully()
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
        BillingPlan BillingPlan = Builder<BillingPlan>.CreateNew()
            .Do(v => v.Daily = new(random.Decimal(), random.Decimal()))
            .Do(v => v.Controlled = new(random.Decimal(), random.Int()))
            .Do(v => v.Free = new(random.Decimal()))
            .Build();

        BillingPlan.AssociateTenant(tenant.Id);
        BillingPlan.AssociateUser(userEmployee);
        BillingPlan.AssociateGroup(group);

        await this.BillingPlanRepository.AddAsync(BillingPlan);

        await this.dbContext.SaveChangesAsync();

        // Act 
        BillingPlan? selectedBillingPlan = await this.BillingPlanRepository.GetByIdAsync(BillingPlan.Id);

        // Assert
        Assert.IsNotNull(selectedBillingPlan);
        Assert.AreEqual(group, selectedBillingPlan.Group);
    }
}
