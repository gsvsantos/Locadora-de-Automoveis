using LocadoraDeAutomoveis.Domain.Configurations;

namespace LocadoraDeAutomoveis.Tests.Unit.Configurations.Domain;

[TestClass]
[TestCategory("Configuration Domain - Unit Tests")]
public sealed class ConfigurationTests
{
    [TestMethod]
    public void ConfigurationConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        Configuration configuration = new();

        // Assert
        Assert.AreNotEqual(Guid.Empty, configuration.Id);
        Assert.IsTrue(configuration.IsActive);
        Assert.AreEqual(0, configuration.GasolinePrice);
        Assert.AreEqual(0, configuration.GasPrice);
        Assert.AreEqual(0, configuration.DieselPrice);
        Assert.AreEqual(0, configuration.AlcoholPrice);
    }

    [TestMethod]
    public void ConfigurationConstructor_Parameterized_ShouldWorks()
    {
        // Arrange & Act
        Configuration configuration = new(
            5.50m,
            3.20m,
            4.90m,
            4.10m
        );

        // Assert
        Assert.AreNotEqual(Guid.Empty, configuration.Id);
        Assert.IsTrue(configuration.IsActive);
        Assert.AreEqual(5.50m, configuration.GasolinePrice);
        Assert.AreEqual(3.20m, configuration.GasPrice);
        Assert.AreEqual(4.90m, configuration.DieselPrice);
        Assert.AreEqual(4.10m, configuration.AlcoholPrice);
    }

    [TestMethod]
    public void ConfigurationMethod_Update_ShouldWorks()
    {
        // Arrange
        Configuration configuration = new(
            1m,
            1m,
            1m,
            1m
        );

        Configuration updatedConfiguration = new(
            6.00m,
            3.50m,
            5.00m,
            4.50m
        );

        // Act
        configuration.Update(updatedConfiguration);

        // Assert
        Assert.AreEqual(6.00m, configuration.GasolinePrice);
        Assert.AreEqual(3.50m, configuration.GasPrice);
        Assert.AreEqual(5.00m, configuration.DieselPrice);
        Assert.AreEqual(4.50m, configuration.AlcoholPrice);
    }
}