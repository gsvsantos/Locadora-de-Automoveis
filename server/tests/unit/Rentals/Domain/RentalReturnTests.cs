using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Domain.Rentals;

namespace LocadoraDeAutomoveis.Tests.Unit.Rentals.Domain;

[TestClass]
[TestCategory("Rental Return Domain - Unit Tests")]
public sealed class RentalReturnTests
{
    [TestMethod]
    public void RentalReturnConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        RentalReturn rentalReturn = new();

        // Assert
        Assert.AreNotEqual(Guid.Empty, rentalReturn.Id);
        Assert.IsTrue(rentalReturn.IsActive);
        Assert.AreEqual(0, rentalReturn.EndKm);
        Assert.AreEqual(0, rentalReturn.TotalMileage);
        Assert.AreEqual(0, rentalReturn.ExtrasTotalCost);
        Assert.AreEqual(0, rentalReturn.FuelPenalty);
        Assert.AreEqual(0, rentalReturn.PenaltyTotalCost);
        Assert.AreEqual(0, rentalReturn.DiscountTotal);
        Assert.AreEqual(0, rentalReturn.FinalPrice);
        Assert.AreEqual(DateTimeOffset.MinValue, rentalReturn.ReturnDate);
    }

    [TestMethod]
    public void RentalReturnConstructor_Parametered_ShouldInitializeProperties()
    {
        // Arrange
        DateTimeOffset returnDate = DateTimeOffset.UtcNow;
        decimal endKm = 1500;
        decimal totalMileage = 500;

        // Act
        RentalReturn rentalReturn = new(returnDate, endKm, totalMileage);

        // Assert
        Assert.AreEqual(returnDate, rentalReturn.ReturnDate);
        Assert.AreEqual(endKm, rentalReturn.EndKm);
        Assert.AreEqual(totalMileage, rentalReturn.TotalMileage);
        Assert.AreNotEqual(Guid.Empty, rentalReturn.Id);
    }

    [TestMethod]
    public void RentalReturnMethod_SetFuelLevel_ShouldUpdateProperty()
    {
        // Arrange
        RentalReturn rentalReturn = new();
        EFuelLevel level = EFuelLevel.Half;

        // Act
        rentalReturn.SetFuelLevel(level);

        // Assert
        Assert.AreEqual(level, rentalReturn.FuelLevelAtReturn);
    }

    [TestMethod]
    public void RentalReturnMethod_SetServicesTotal_ShouldUpdateProperty()
    {
        // Arrange
        RentalReturn rentalReturn = new();
        decimal servicesTotal = 250.50m;

        // Act
        rentalReturn.SetExtrasTotalCost(servicesTotal);

        // Assert
        Assert.AreEqual(servicesTotal, rentalReturn.ExtrasTotalCost);
    }

    [TestMethod]
    public void RentalReturnMethod_SetFuelPenalty_ShouldUpdateProperty()
    {
        // Arrange
        RentalReturn rentalReturn = new();
        decimal fuelPenalty = 150.00m;

        // Act
        rentalReturn.SetFuelPenalty(fuelPenalty);

        // Assert
        Assert.AreEqual(fuelPenalty, rentalReturn.FuelPenalty);
    }

    [TestMethod]
    public void RentalReturnMethod_SetPenaltyTotal_ShouldUpdateProperty()
    {
        // Arrange
        RentalReturn rentalReturn = new();
        decimal penaltyTotal = 300.00m;

        // Act
        rentalReturn.SetPenaltyTotal(penaltyTotal);

        // Assert
        Assert.AreEqual(penaltyTotal, rentalReturn.PenaltyTotalCost);
    }

    [TestMethod]
    public void RentalReturnMethod_SetDiscountTotal_ShouldUpdateProperty()
    {
        // Arrange
        RentalReturn rentalReturn = new();
        decimal discount = 250.00m;

        // Act
        rentalReturn.SetDiscountTotal(discount);

        // Assert
        Assert.AreEqual(discount, rentalReturn.DiscountTotal);
    }

    [TestMethod]
    public void RentalReturnMethod_SetFinalPrice_ShouldUpdateProperty()
    {
        // Arrange
        RentalReturn rentalReturn = new();
        decimal finalPrice = 1250.00m;

        // Act
        rentalReturn.SetFinalPrice(finalPrice);

        // Assert
        Assert.AreEqual(finalPrice, rentalReturn.FinalPrice);
    }

    [TestMethod]
    public void RentalReturnMethod_AssociateRental_ShouldWorks()
    {
        // Arrange
        RentalReturn rentalReturn = new();
        Rental rental = Builder<Rental>.CreateNew().Build();

        // Act
        rentalReturn.AssociateRental(rental);

        // Assert
        Assert.AreEqual(rental.Id, rentalReturn.RentalId);
        Assert.AreEqual(rental, rentalReturn.Rental);
    }

    [TestMethod]
    public void RentalReturnMethod_Update_ShouldDoNothing()
    {
        // Arrange
        RentalReturn originalReturn = new(DateTimeOffset.Now, 1000, 100);
        RentalReturn updateData = new(DateTimeOffset.Now.AddDays(1), 2000, 200);

        // Act
        originalReturn.Update(updateData);

        // Assert
        Assert.AreNotEqual(updateData.ReturnDate, originalReturn.ReturnDate);
        Assert.AreNotEqual(updateData.EndKm, originalReturn.EndKm);
        Assert.AreNotEqual(updateData.TotalMileage, originalReturn.TotalMileage);
    }
}