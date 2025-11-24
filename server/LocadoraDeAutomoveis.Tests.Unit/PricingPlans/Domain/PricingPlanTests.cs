using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.PricingPlans;

namespace LocadoraDeAutomoveis.Tests.Unit.PricingPlans.Domain;

[TestClass]
[TestCategory("PricingPlan Domain - Unit Tests")]
public class PricingPlanTests
{
    [TestMethod]
    public void PricingPlanConstructor_Default_ShouldWorks()
    {
        // Arrange & Act
        PricingPlan pricingPlan = new();

        // Assert
        Assert.AreNotEqual(Guid.Empty, pricingPlan.Id);
        Assert.IsNull(pricingPlan.DailyPlan);
        Assert.IsNull(pricingPlan.ControlledPlan);
        Assert.IsNull(pricingPlan.FreePlan);
        Assert.AreEqual(Guid.Empty, pricingPlan.GroupId);
        Assert.IsNull(pricingPlan.Group);
    }

    [TestMethod]
    public void PricingPlanConstructor_Parameterized_ShouldWorks()
    {
        // Arrange & Act
        DailyPlanProps dailyPlan = new(100m, 2m);
        ControlledPlanProps controlledPlan = new(80m, 3m, 200);
        FreePlanProps freePlan = new(70m);

        PricingPlan pricingPlan = new(
            "SUV Plan", dailyPlan, controlledPlan, freePlan
        );

        // Assert
        Assert.AreNotEqual(Guid.Empty, pricingPlan.Id);
        Assert.AreEqual("SUV Plan", pricingPlan.Name);
        Assert.AreEqual(dailyPlan, pricingPlan.DailyPlan);
        Assert.AreEqual(controlledPlan, pricingPlan.ControlledPlan);
        Assert.AreEqual(freePlan, pricingPlan.FreePlan);
        Assert.AreEqual(Guid.Empty, pricingPlan.GroupId);
        Assert.IsNull(pricingPlan.Group);
    }

    [TestMethod]
    public void PricingPlanMethod_Update_ShouldWorks()
    {
        // Arrange
        DailyPlanProps dailyPlan1 = new(100m, 2m);
        ControlledPlanProps controlledPlan1 = new(80m, 3m, 200);
        FreePlanProps freePlan1 = new(70m);

        PricingPlan pricingPlan1 = new(
            "SuuuV Plan", dailyPlan1, controlledPlan1, freePlan1
        );

        DailyPlanProps dailyPlan2 = new(120m, 2.5m);
        ControlledPlanProps controlledPlan2 = new(90m, 3.5m, 250);
        FreePlanProps freePlan2 = new(75m);

        PricingPlan pricingPlan2 = new(
            "SUV Plan", dailyPlan2, controlledPlan2, freePlan2
        );

        // Act
        pricingPlan1.Update(pricingPlan2);

        // Assert
        Assert.AreEqual(pricingPlan2.Name, pricingPlan1.Name);
        Assert.AreEqual(dailyPlan2, pricingPlan1.DailyPlan);
        Assert.AreEqual(controlledPlan2, pricingPlan1.ControlledPlan);
        Assert.AreEqual(freePlan2, pricingPlan1.FreePlan);
    }

    [TestMethod]
    public void PricingPlanMethod_AssociateGroup_ShouldWorks()
    {
        // Arrange
        PricingPlan pricingPlan = new(
            "SUV Plan",
            new DailyPlanProps(100m, 2m),
            new ControlledPlanProps(80m, 3m, 200),
            new FreePlanProps(70m)
        );
        Group group = new("Group A");

        // Act
        pricingPlan.AssociateGroup(group);

        // Assert
        Assert.AreEqual(group.Id, pricingPlan.GroupId);
        Assert.AreEqual("SUV Plan", pricingPlan.Name);
        Assert.AreEqual(group, pricingPlan.Group);
        Assert.IsTrue(group.PricingPlans.Contains(pricingPlan));
    }

    [TestMethod]
    public void PricingPlanMethod_DisassociateGroup_ShouldWorks()
    {
        // Arrange
        PricingPlan pricingPlan = new(
            "SUV Plan",
            new DailyPlanProps(100m, 2m),
            new ControlledPlanProps(80m, 3m, 200),
            new FreePlanProps(70m)
        );

        Group group = new("Group A");

        pricingPlan.AssociateGroup(group);

        // Act
        pricingPlan.DisassociateGroup();

        // Assert
        Assert.AreEqual(Guid.Empty, pricingPlan.GroupId);
        Assert.AreEqual("SUV Plan", pricingPlan.Name);
        Assert.IsNull(pricingPlan.Group);
        Assert.IsFalse(group.PricingPlans.Contains(pricingPlan));
    }
}
