using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Groups;

namespace LocadoraDeAutomoveis.Tests.Unit.BillingPlans.Domain;

[TestClass]
[TestCategory("BillingPlan Domain - Unit Tests")]
public class BillingPlanTests
{
    [TestMethod]
    public void BillingPlanConstructor_Default_ShouldWorks()
    {
        // Arrange & Act
        BillingPlan billingPlan = new();

        // Assert
        Assert.AreNotEqual(Guid.Empty, billingPlan.Id);
        Assert.IsNull(billingPlan.Daily);
        Assert.IsNull(billingPlan.Controlled);
        Assert.IsNull(billingPlan.Free);
        Assert.AreEqual(Guid.Empty, billingPlan.GroupId);
        Assert.IsNull(billingPlan.Group);
    }

    [TestMethod]
    public void BillingPlanConstructor_Parameterized_ShouldWorks()
    {
        // Arrange & Act
        DailyBilling dailyPlan = new(100m, 2m);
        ControlledBilling controlledPlan = new(80m, 200);
        FreeBilling freePlan = new(70m);

        BillingPlan BillingPlan = new(
            "SUV Plan", dailyPlan, controlledPlan, freePlan
        );

        // Assert
        Assert.AreNotEqual(Guid.Empty, BillingPlan.Id);
        Assert.AreEqual("SUV Plan", BillingPlan.Name);
        Assert.AreEqual(dailyPlan, BillingPlan.Daily);
        Assert.AreEqual(controlledPlan, BillingPlan.Controlled);
        Assert.AreEqual(freePlan, BillingPlan.Free);
        Assert.AreEqual(Guid.Empty, BillingPlan.GroupId);
        Assert.IsNull(BillingPlan.Group);
    }

    [TestMethod]
    public void BillingPlanMethod_Update_ShouldWorks()
    {
        // Arrange
        DailyBilling dailyPlan1 = new(100m, 2m);
        ControlledBilling controlledPlan1 = new(80m, 200);
        FreeBilling freePlan1 = new(70m);

        BillingPlan BillingPlan1 = new(
            "SuuuV Plan", dailyPlan1, controlledPlan1, freePlan1
        );

        DailyBilling dailyPlan2 = new(120m, 2.5m);
        ControlledBilling controlledPlan2 = new(90m, 250);
        FreeBilling freePlan2 = new(75m);

        BillingPlan BillingPlan2 = new(
            "SUV Plan", dailyPlan2, controlledPlan2, freePlan2
        );

        // Act
        BillingPlan1.Update(BillingPlan2);

        // Assert
        Assert.AreEqual(BillingPlan2.Name, BillingPlan1.Name);
        Assert.AreEqual(dailyPlan2, BillingPlan1.Daily);
        Assert.AreEqual(controlledPlan2, BillingPlan1.Controlled);
        Assert.AreEqual(freePlan2, BillingPlan1.Free);
    }

    [TestMethod]
    public void BillingPlanMethod_AssociateGroup_ShouldWorks()
    {
        // Arrange
        BillingPlan BillingPlan = new(
            "SUV Plan",
            new DailyBilling(100m, 2m),
            new ControlledBilling(80m, 200),
            new FreeBilling(70m)
        );
        Group group = new("Group A");

        // Act
        BillingPlan.AssociateGroup(group);

        // Assert
        Assert.AreEqual(group.Id, BillingPlan.GroupId);
        Assert.AreEqual("SUV Plan", BillingPlan.Name);
        Assert.AreEqual(group, BillingPlan.Group);
        Assert.IsTrue(group.BillingPlans.Contains(BillingPlan));
    }

    [TestMethod]
    public void BillingPlanMethod_AssociateGroup_ShouldReturn()
    {
        // Arrange
        BillingPlan BillingPlan = new(
            "SUV Plan",
            new DailyBilling(100m, 2m),
            new ControlledBilling(80m, 200),
            new FreeBilling(70m)
        );
        Group group = new("Group A");
        BillingPlan.AssociateGroup(group);

        // Act
        BillingPlan.AssociateGroup(group);

        // Assert
        Assert.AreEqual(group.Id, BillingPlan.GroupId);
        Assert.AreEqual("SUV Plan", BillingPlan.Name);
        Assert.AreEqual(group, BillingPlan.Group);
        Assert.IsTrue(group.BillingPlans.Contains(BillingPlan));
    }

    [TestMethod]
    public void BillingPlanMethod_AssociateGroup_ShouldDisassociateBefore_And_Work()
    {
        // Arrange
        BillingPlan BillingPlan = new(
            "SUV Plan",
            new DailyBilling(100m, 2m),
            new ControlledBilling(80m, 200),
            new FreeBilling(70m)
        );
        Group group1 = new("Group A");
        BillingPlan.AssociateGroup(group1);

        Group group2 = new("Group B");

        // Act
        BillingPlan.AssociateGroup(group2);

        // Assert
        Assert.AreEqual(group2.Id, BillingPlan.GroupId);
        Assert.AreEqual("SUV Plan", BillingPlan.Name);
        Assert.AreEqual(group2, BillingPlan.Group);
        Assert.IsTrue(group2.BillingPlans.Contains(BillingPlan));
    }

    [TestMethod]
    public void BillingPlanMethod_DisassociateGroup_ShouldNotWork()
    {
        // Arrange
        BillingPlan BillingPlan = new(
            "SUV Plan",
            new DailyBilling(100m, 2m),
            new ControlledBilling(80m, 200),
            new FreeBilling(70m)
        );

        // Act
        BillingPlan.DisassociateGroup();

        // Assert
        Assert.AreEqual(Guid.Empty, BillingPlan.GroupId);
        Assert.AreEqual("SUV Plan", BillingPlan.Name);
        Assert.IsNull(BillingPlan.Group);
    }

    [TestMethod]
    public void BillingPlanMethod_DisassociateGroup_ShouldWorks()
    {
        // Arrange
        BillingPlan BillingPlan = new(
            "SUV Plan",
            new DailyBilling(100m, 2m),
            new ControlledBilling(80m, 200),
            new FreeBilling(70m)
        );

        Group group = new("Group A");

        BillingPlan.AssociateGroup(group);

        // Act
        BillingPlan.DisassociateGroup();

        // Assert
        Assert.AreEqual(Guid.Empty, BillingPlan.GroupId);
        Assert.AreEqual("SUV Plan", BillingPlan.Name);
        Assert.IsNull(BillingPlan.Group);
        Assert.IsFalse(group.BillingPlans.Contains(BillingPlan));
    }

}
