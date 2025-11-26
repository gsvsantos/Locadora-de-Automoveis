using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Domain.RateServices;

namespace LocadoraDeAutomoveis.Tests.Unit.RateServices.Domain;

[TestClass]
[TestCategory("RateService Domain - Unit Tests")]
public sealed class RateServiceTests
{
    [TestMethod]
    public void RateServiceConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        RateService rateService = new();

        // Assert
        Assert.AreNotEqual(Guid.Empty, rateService.Id);
        Assert.IsTrue(rateService.IsActive);
        Assert.AreEqual(string.Empty, rateService.Name);
        Assert.AreEqual(0, rateService.Price);
        Assert.IsFalse(rateService.IsChargedPerDay);
        Assert.AreEqual(ERateType.Generic, rateService.RateType);
    }

    [TestMethod]
    public void RateServiceConstructor_WithParams_ShouldInitializeProperties()
    {
        // Arrange & Act
        RateService rateService = new(
            "GPS",
            15.50m
        );

        // Assert
        Assert.AreNotEqual(Guid.Empty, rateService.Id);
        Assert.IsTrue(rateService.IsActive);
        Assert.AreEqual("GPS", rateService.Name);
        Assert.AreEqual(15.50m, rateService.Price);
        Assert.IsFalse(rateService.IsChargedPerDay);
        Assert.AreEqual(ERateType.Generic, rateService.RateType);
    }

    [TestMethod]
    public void RateServiceMethod_Update_ShouldWorks()
    {
        // Arrange
        RateService rateService = new(
            "GPS",
            10.00m
        );

        RateService rateService2 = new("Cadeira de Bebê", 20.00m);
        rateService2.MarkAsFixed();

        // Act
        rateService.Update(rateService2);

        // Assert
        Assert.AreNotEqual(Guid.Empty, rateService.Id);
        Assert.IsTrue(rateService.IsActive);
        Assert.AreEqual("Cadeira de Bebê", rateService.Name);
        Assert.AreEqual(20.00m, rateService.Price);
        Assert.IsTrue(rateService.IsChargedPerDay);
    }

    [TestMethod]
    public void RateServiceMethod_MarkAsFixed_ShouldWorks()
    {
        // Arrange
        RateService rateService = Builder<RateService>.CreateNew().Build();

        // Act
        rateService.MarkAsFixed();

        // Assert
        Assert.IsTrue(rateService.IsChargedPerDay);
    }

    [TestMethod]
    public void RateServiceMethod_MarkAsDaily_ShouldWorks()
    {
        // Arrange
        RateService rateService = Builder<RateService>.CreateNew().Build();

        rateService.MarkAsFixed();

        // Act
        rateService.MarkAsDaily();

        // Assert
        Assert.IsFalse(rateService.IsChargedPerDay);
    }

    [TestMethod]
    public void RateServiceMethod_DefineRateType_ShouldWorks()
    {
        // Arrange
        RateService rateService = Builder<RateService>.CreateNew().Build();
        ERateType newType = ERateType.InsuranceClient;

        // Act
        rateService.DefineRateType(newType);

        // Assert
        Assert.AreEqual(newType, rateService.RateType);
    }
}