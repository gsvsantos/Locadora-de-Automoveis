using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Domain.RentalExtras;

namespace LocadoraDeAutomoveis.Tests.Unit.RentalExtras.Domain;

[TestClass]
[TestCategory("RentalExtra Domain - Unit Tests")]
public sealed class RentalExtraTests
{
    [TestMethod]
    public void RentalExtraConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        RentalExtra rentalExtra = new();

        // Assert
        Assert.AreNotEqual(Guid.Empty, rentalExtra.Id);
        Assert.IsTrue(rentalExtra.IsActive);
        Assert.AreEqual(string.Empty, rentalExtra.Name);
        Assert.AreEqual(0, rentalExtra.Price);
        Assert.IsFalse(rentalExtra.IsDaily);
        Assert.AreEqual(EExtraType.Equipment, rentalExtra.Type);
    }

    [TestMethod]
    public void RentalExtraConstructor_WithParams_ShouldInitializeProperties()
    {
        // Arrange & Act
        RentalExtra rentalExtra = new(
            "GPS",
            15.50m
        );

        // Assert
        Assert.AreNotEqual(Guid.Empty, rentalExtra.Id);
        Assert.IsTrue(rentalExtra.IsActive);
        Assert.AreEqual("GPS", rentalExtra.Name);
        Assert.AreEqual(15.50m, rentalExtra.Price);
        Assert.IsFalse(rentalExtra.IsDaily);
        Assert.AreEqual(EExtraType.Equipment, rentalExtra.Type);
    }

    [TestMethod]
    public void RentalExtraMethod_Update_ShouldWorks()
    {
        // Arrange
        RentalExtra rentalExtra = new(
            "GPS",
            10.00m
        );

        RentalExtra rentalExtra2 = new("Cadeira de Bebê", 20.00m);
        rentalExtra2.MarkAsFixed();

        // Act
        rentalExtra.Update(rentalExtra2);

        // Assert
        Assert.AreNotEqual(Guid.Empty, rentalExtra.Id);
        Assert.IsTrue(rentalExtra.IsActive);
        Assert.AreEqual("Cadeira de Bebê", rentalExtra.Name);
        Assert.AreEqual(20.00m, rentalExtra.Price);
        Assert.IsTrue(rentalExtra.IsDaily);
    }

    [TestMethod]
    public void RentalExtraMethod_MarkAsFixed_ShouldWorks()
    {
        // Arrange
        RentalExtra rentalExtra = Builder<RentalExtra>.CreateNew().Build();

        // Act
        rentalExtra.MarkAsFixed();

        // Assert
        Assert.IsTrue(rentalExtra.IsDaily);
    }

    [TestMethod]
    public void RentalExtraMethod_MarkAsDaily_ShouldWorks()
    {
        // Arrange
        RentalExtra rentalExtra = Builder<RentalExtra>.CreateNew().Build();

        rentalExtra.MarkAsFixed();

        // Act
        rentalExtra.MarkAsDaily();

        // Assert
        Assert.IsFalse(rentalExtra.IsDaily);
    }

    [TestMethod]
    public void RentalExtraMethod_DefineType_ShouldWorks()
    {
        // Arrange
        RentalExtra rentalExtra = Builder<RentalExtra>.CreateNew().Build();
        EExtraType newType = EExtraType.Service;

        // Act
        rentalExtra.DefineType(newType);

        // Assert
        Assert.AreEqual(newType, rentalExtra.Type);
    }
}