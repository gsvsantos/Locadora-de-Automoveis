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
        Assert.IsNull(billingPlan.DailyPlan);
        Assert.IsNull(billingPlan.ControlledPlan);
        Assert.IsNull(billingPlan.FreePlan);
        Assert.AreEqual(Guid.Empty, billingPlan.GroupId);
        Assert.IsNull(billingPlan.Group);
    }

    [TestMethod]
    public void BillingPlanConstructor_Parameterized_ShouldWorks()
    {
        // Arrange & Act
        DailyPlanProps dailyPlan = new(100m, 2m);
        ControlledPlanProps controlledPlan = new(80m, 200);
        FreePlanProps freePlan = new(70m);

        BillingPlan BillingPlan = new(
            "SUV Plan", dailyPlan, controlledPlan, freePlan
        );

        // Assert
        Assert.AreNotEqual(Guid.Empty, BillingPlan.Id);
        Assert.AreEqual("SUV Plan", BillingPlan.Name);
        Assert.AreEqual(dailyPlan, BillingPlan.DailyPlan);
        Assert.AreEqual(controlledPlan, BillingPlan.ControlledPlan);
        Assert.AreEqual(freePlan, BillingPlan.FreePlan);
        Assert.AreEqual(Guid.Empty, BillingPlan.GroupId);
        Assert.IsNull(BillingPlan.Group);
    }

    [TestMethod]
    public void BillingPlanMethod_Update_ShouldWorks()
    {
        // Arrange
        DailyPlanProps dailyPlan1 = new(100m, 2m);
        ControlledPlanProps controlledPlan1 = new(80m, 200);
        FreePlanProps freePlan1 = new(70m);

        BillingPlan BillingPlan1 = new(
            "SuuuV Plan", dailyPlan1, controlledPlan1, freePlan1
        );

        DailyPlanProps dailyPlan2 = new(120m, 2.5m);
        ControlledPlanProps controlledPlan2 = new(90m, 250);
        FreePlanProps freePlan2 = new(75m);

        BillingPlan BillingPlan2 = new(
            "SUV Plan", dailyPlan2, controlledPlan2, freePlan2
        );

        // Act
        BillingPlan1.Update(BillingPlan2);

        // Assert
        Assert.AreEqual(BillingPlan2.Name, BillingPlan1.Name);
        Assert.AreEqual(dailyPlan2, BillingPlan1.DailyPlan);
        Assert.AreEqual(controlledPlan2, BillingPlan1.ControlledPlan);
        Assert.AreEqual(freePlan2, BillingPlan1.FreePlan);
    }

    [TestMethod]
    public void BillingPlanMethod_AssociateGroup_ShouldWorks()
    {
        // Arrange
        BillingPlan BillingPlan = new(
            "SUV Plan",
            new DailyPlanProps(100m, 2m),
            new ControlledPlanProps(80m, 200),
            new FreePlanProps(70m)
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
    public void BillingPlanMethod_DisassociateGroup_ShouldWorks()
    {
        // Arrange
        BillingPlan BillingPlan = new(
            "SUV Plan",
            new DailyPlanProps(100m, 2m),
            new ControlledPlanProps(80m, 200),
            new FreePlanProps(70m)
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
