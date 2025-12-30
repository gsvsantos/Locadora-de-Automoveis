using FizzWare.NBuilder;

using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Configurations;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Vehicles;

namespace LocadoraDeAutomoveis.Tests.Unit.Rentals.Domain;

[TestClass]
[TestCategory("Rental Calculator Domain - Unit Tests")]
public sealed class RentalCalculatorTests
{
    [TestMethod]
    public void RentalCalculatorMethod_CalculateBasePrice_StartEqualsEnd_ShouldFallbackToMinimumOneDay()
    {
        // Arrange
        DateTimeOffset now = new(2025, 01, 01, 10, 00, 00, TimeSpan.Zero);

        Rental rental = new(now, now);

        DailyBilling dailyBilling = new(100m, 0m);
        BillingPlan billingPlan = new(
            "Plano Diário",
            dailyBilling,
            new ControlledBilling(0m, 0m),
            new FreeBilling(0m)
        );

        rental.AssociateBillingPlan(billingPlan);
        rental.SetBillingPlanType(EBillingPlanType.Daily);

        // Act
        decimal basePrice = RentalCalculator.CalculateBasePrice(rental);

        // Assert
        Assert.AreEqual(100m, basePrice);
    }

    [TestMethod]
    public void RentalCalculatorMethod_CalculateBasePrice_DailyPlan_WithLessThanOneDay_ShouldChargeOneDay()
    {
        // Arrange
        DateTimeOffset startDate = new(2025, 01, 01, 10, 00, 00, TimeSpan.Zero);
        DateTimeOffset expectedReturnDate = startDate.AddHours(1);

        DailyBilling dailyBilling = new(100m, 2m);
        ControlledBilling controlledBilling = new(0m, 0m);
        FreeBilling freeBilling = new(0m);

        BillingPlan billingPlan = new(
            "Plano Diário",
            dailyBilling,
            controlledBilling,
            freeBilling
        );

        Rental rental = new(startDate, expectedReturnDate);
        rental.AssociateBillingPlan(billingPlan);
        rental.SetBillingPlanType(EBillingPlanType.Daily);

        // Act
        decimal basePrice = RentalCalculator.CalculateBasePrice(rental);

        // Assert
        decimal expectedBasePrice = 100m * 1;
        Assert.AreEqual(expectedBasePrice, basePrice);
    }

    [TestMethod]
    public void RentalCalculatorMethod_CalculateBasePrice_DailyPlan_WithExtras_ShouldIncludeExtrasCost()
    {
        // Arrange
        DateTimeOffset startDate = new(2025, 01, 01, 10, 00, 00, TimeSpan.Zero);
        DateTimeOffset expectedReturnDate = startDate.AddDays(3);

        DailyBilling dailyBilling = new(100m, 0m);
        ControlledBilling controlledBilling = new(0m, 0m);
        FreeBilling freeBilling = new(0m);

        BillingPlan billingPlan = new(
            "Plano Diário",
            dailyBilling,
            controlledBilling,
            freeBilling
        );

        RentalExtra dailyExtra = Builder<RentalExtra>.CreateNew()
            .With(e => e.IsDaily = true)
            .With(e => e.Price = 10m)
            .Build();

        RentalExtra fixedExtra = Builder<RentalExtra>.CreateNew()
            .With(e => e.IsDaily = false)
            .With(e => e.Price = 25m)
            .Build();

        Rental rental = new(startDate, expectedReturnDate);
        rental.AssociateBillingPlan(billingPlan);
        rental.SetBillingPlanType(EBillingPlanType.Daily);
        rental.AddExtra(dailyExtra);
        rental.AddExtra(fixedExtra);

        // Act
        decimal basePrice = RentalCalculator.CalculateBasePrice(rental);

        // Assert
        decimal expectedPlanTotal = 100m * 3;
        decimal expectedExtrasTotal = (10m * 3) + 25m;
        decimal expectedBasePrice = expectedPlanTotal + expectedExtrasTotal;

        Assert.AreEqual(expectedBasePrice, basePrice);
    }

    [TestMethod]
    public void RentalCalculatorMethod_CalculateBasePrice_FreePlan_WithLessThanOneDay_ShouldChargeOneDay()
    {
        // Arrange
        DateTimeOffset startDate = new(2025, 01, 01, 10, 00, 00, TimeSpan.Zero);
        DateTimeOffset expectedReturnDate = startDate.AddHours(1);

        DailyBilling dailyBilling = new(0m, 0m);
        ControlledBilling controlledBilling = new(0m, 0m);
        FreeBilling freeBilling = new(100m);

        BillingPlan billingPlan = new(
            "Plano Diário",
            dailyBilling,
            controlledBilling,
            freeBilling
        );

        Rental rental = new(startDate, expectedReturnDate);
        rental.AssociateBillingPlan(billingPlan);
        rental.SetBillingPlanType(EBillingPlanType.Free);

        // Act
        decimal basePrice = RentalCalculator.CalculateBasePrice(rental);

        // Assert
        decimal expectedBasePrice = 100m * 1;
        Assert.AreEqual(expectedBasePrice, basePrice);
    }

    [TestMethod]
    public void RentalCalculatorMethod_CalculateBasePrice_UnknownPlan_WithLessThanOneDay_ShouldReturnZero()
    {
        // Arrange
        DateTimeOffset startDate = new(2025, 01, 01, 10, 00, 00, TimeSpan.Zero);
        DateTimeOffset expectedReturnDate = startDate.AddHours(1);

        DailyBilling dailyBilling = new(0m, 0m);
        ControlledBilling controlledBilling = new(0m, 0m);
        FreeBilling freeBilling = new(100m);

        BillingPlan billingPlan = new(
            "Plano Diário",
            dailyBilling,
            controlledBilling,
            freeBilling
        );

        Rental rental = new(startDate, expectedReturnDate);
        rental.AssociateBillingPlan(billingPlan);
        rental.SetBillingPlanType((EBillingPlanType)999);

        // Act
        decimal basePrice = RentalCalculator.CalculateBasePrice(rental);

        // Assert
        Assert.AreEqual(0m, basePrice);
    }

    [TestMethod]
    public void RentalCalculatorMethod_CalculateBasePrice_WithCouponDiscount_ShouldApplyDiscount_AndNeverBelowZero()
    {
        // Arrange
        DateTimeOffset startDate = new(2025, 01, 01, 10, 00, 00, TimeSpan.Zero);
        DateTimeOffset expectedReturnDate = startDate.AddDays(1);

        DailyBilling dailyBilling = new(50m, 0m);
        ControlledBilling controlledBilling = new(0m, 0m);
        FreeBilling freeBilling = new(0m);

        BillingPlan billingPlan = new(
            "Plano Diário",
            dailyBilling,
            controlledBilling,
            freeBilling
        );

        Coupon coupon = Builder<Coupon>.CreateNew()
            .With(c => c.DiscountValue = 9999m)
            .Build();

        Rental rental = new(startDate, expectedReturnDate);
        rental.AssociateBillingPlan(billingPlan);
        rental.SetBillingPlanType(EBillingPlanType.Daily);
        rental.AssociateCoupon(coupon);

        // Act
        decimal basePrice = RentalCalculator.CalculateBasePrice(rental);

        // Assert
        Assert.AreEqual(0m, basePrice);
    }

    [TestMethod]
    public void RentalCalculatorMethod_CalculateBasePrice_WhenExtrasListIsNull_ShouldWorks()
    {
        // Arrange
        DateTimeOffset startDate = new(2025, 01, 01, 10, 00, 00, TimeSpan.Zero);
        DateTimeOffset expectedReturnDate = startDate.AddDays(2);

        DailyBilling dailyBilling = new(100m, 0m);
        ControlledBilling controlledBilling = new(0m, 0m);
        FreeBilling freeBilling = new(0m);

        BillingPlan billingPlan = new(
            "Plano Diário",
            dailyBilling,
            controlledBilling,
            freeBilling
        );

        Rental rental = new(startDate, expectedReturnDate);
        rental.AssociateBillingPlan(billingPlan);
        rental.SetBillingPlanType(EBillingPlanType.Daily);
        rental.Extras = null!;

        // Act
        decimal basePrice = RentalCalculator.CalculateBasePrice(rental);

        // Assert
        decimal expectedBasePrice = 100m * 2;
        Assert.AreEqual(expectedBasePrice, basePrice);
    }

    [TestMethod]
    public void RentalCalculatorMethod_CalculateFinalPrice_DailyPlan_WithFuelPenalty_DelayPenalty_AndDiscount_ShouldWorks()
    {
        // Arrange
        DateTimeOffset startDate = new(2025, 01, 01, 10, 00, 00, TimeSpan.Zero);
        DateTimeOffset expectedReturnDate = startDate.AddDays(2);
        DateTimeOffset actualReturnDate = startDate.AddDays(3);

        DailyBilling dailyBilling = new(100m, 2m);
        ControlledBilling controlledBilling = new(0m, 0m);
        FreeBilling freeBilling = new(0m);

        BillingPlan billingPlan = new(
            "Plano Diário",
            dailyBilling,
            controlledBilling,
            freeBilling
        );

        RentalExtra dailyExtra = Builder<RentalExtra>.CreateNew()
            .With(e => e.IsDaily = true)
            .With(e => e.Price = 10m)
            .Build();

        RentalExtra fixedExtra = Builder<RentalExtra>.CreateNew()
            .With(e => e.IsDaily = false)
            .With(e => e.Price = 20m)
            .Build();

        Coupon coupon = Builder<Coupon>.CreateNew()
            .With(c => c.DiscountValue = 100m)
            .Build();

        Vehicle vehicle = Builder<Vehicle>.CreateNew().Build();
        vehicle.Kilometers = 10000m;
        vehicle.FuelTankCapacity = 50;
        vehicle.SetFuelType(EFuelType.Gasoline);

        Configuration config = new(6m, 0m, 0m, 0m);

        Rental rental = new(startDate, expectedReturnDate);
        rental.AssociateBillingPlan(billingPlan);
        rental.SetBillingPlanType(EBillingPlanType.Daily);
        rental.AssociateCoupon(coupon);
        rental.AssociateVehicle(vehicle);
        rental.SetStartKm(1000m);
        rental.AddExtra(dailyExtra);
        rental.AddExtra(fixedExtra);

        // Act
        CalculationResult result = RentalCalculator.CalculateFinalPrice(
            rental,
            actualReturnDate,
            1120m,
            EFuelLevel.Half,
            config
        );

        // Assert
        int daysUsed = 3;
        decimal kilometersDriven = 120m;

        decimal expectedPlanTotal = (100m * daysUsed) + (2m * kilometersDriven);
        decimal expectedServicesTotal = (10m * daysUsed) + 20m;

        decimal expectedFuelPenalty = 50m * ((100 - 50) / 100m) * 6m;
        decimal expectedDelayPenalty = expectedPlanTotal * 0.10m;
        decimal expectedPenaltiesTotal = expectedFuelPenalty + expectedDelayPenalty;

        decimal expectedFinalPrice = (expectedPlanTotal + expectedServicesTotal + expectedPenaltiesTotal) - 100m;

        Assert.AreEqual(expectedFinalPrice, result.FinalPrice);
        Assert.AreEqual(expectedPlanTotal, result.PlanTotal);
        Assert.AreEqual(expectedServicesTotal, result.ServicesTotal);
        Assert.AreEqual(expectedPenaltiesTotal, result.PenaltiesTotal);
        Assert.AreEqual(100m, result.DiscountTotal);
        Assert.AreEqual(expectedFuelPenalty, result.FuelPenalty);

        Assert.AreEqual(10000m + kilometersDriven, rental.Vehicle.Kilometers);
    }

    [TestMethod]
    public void RentalCalculatorMethod_CalculateFinalPrice_ControlledPlan_WithExtrapolatedKilometers_ShouldWorks()
    {
        // Arrange
        DateTimeOffset startDate = new(2025, 01, 01, 10, 00, 00, TimeSpan.Zero);
        DateTimeOffset expectedReturnDate = startDate.AddDays(2);
        DateTimeOffset actualReturnDate = startDate.AddDays(2);

        DailyBilling dailyBilling = new(0m, 0m);
        ControlledBilling controlledBilling = new(100m, 3m);
        FreeBilling freeBilling = new(0m);

        BillingPlan billingPlan = new(
            "Plano Controlado",
            dailyBilling,
            controlledBilling,
            freeBilling
        );

        Vehicle vehicle = Builder<Vehicle>.CreateNew().Build();
        vehicle.Kilometers = 5000m;
        vehicle.FuelTankCapacity = 50;
        vehicle.SetFuelType(EFuelType.Gasoline);

        Configuration config = new(6m, 0m, 0m, 0m);

        Rental rental = new(startDate, expectedReturnDate);
        rental.AssociateBillingPlan(billingPlan);
        rental.SetBillingPlanType(EBillingPlanType.Controlled);
        rental.AssociateVehicle(vehicle);
        rental.SetStartKm(1000m);
        rental.SetEstimatedKilometers(100m);

        // Act
        CalculationResult result = RentalCalculator.CalculateFinalPrice(
            rental,
            actualReturnDate,
            1150m,
            EFuelLevel.Full,
            config
        );

        // Assert
        int daysUsed = 2;
        decimal kilometersDriven = 150m;

        decimal expectedPlanTotal = (100m * daysUsed) + ((kilometersDriven - 100m) * 3m);
        Assert.AreEqual(expectedPlanTotal, result.PlanTotal);
        Assert.AreEqual(expectedPlanTotal, result.FinalPrice);

        Assert.AreEqual(5000m + kilometersDriven, rental.Vehicle.Kilometers);
    }

    [TestMethod]
    public void RentalCalculatorMethod_CalculateFinalPrice_WhenEndKmIsLowerThanStartKm_ShouldNotChargeNegativeKilometers()
    {
        // Arrange
        DateTimeOffset startDate = new(2025, 01, 01, 10, 00, 00, TimeSpan.Zero);
        DateTimeOffset expectedReturnDate = startDate.AddDays(1);

        DailyBilling dailyBilling = new(100m, 10m);
        ControlledBilling controlledBilling = new(0m, 0m);
        FreeBilling freeBilling = new(0m);

        BillingPlan billingPlan = new(
            "Plano Diário",
            dailyBilling,
            controlledBilling,
            freeBilling
        );

        Vehicle vehicle = Builder<Vehicle>.CreateNew().Build();
        vehicle.Kilometers = 1000m;
        vehicle.FuelTankCapacity = 50;
        vehicle.SetFuelType(EFuelType.Gasoline);

        Configuration config = new(6m, 0m, 0m, 0m);

        Rental rental = new(startDate, expectedReturnDate);
        rental.AssociateBillingPlan(billingPlan);
        rental.SetBillingPlanType(EBillingPlanType.Daily);
        rental.AssociateVehicle(vehicle);
        rental.SetStartKm(500m);

        // Act
        CalculationResult result = RentalCalculator.CalculateFinalPrice(
            rental,
            expectedReturnDate,
            100m,
            EFuelLevel.Full,
            config
        );

        // Assert
        decimal expectedPlanTotal = 100m * 1;
        Assert.AreEqual(expectedPlanTotal, result.PlanTotal);
        Assert.AreEqual(1000m, rental.Vehicle.Kilometers);
    }

    [TestMethod]
    public void RentalCalculatorMethod_CalculateFinalPrice_WhenReturnIsSameDay_ShouldNotApplyDelayPenalty()
    {
        // Arrange
        DateTimeOffset startDate = new(2025, 01, 01, 10, 00, 00, TimeSpan.Zero);
        DateTimeOffset expectedReturnDate = startDate.AddDays(1);
        DateTimeOffset actualReturnDate = expectedReturnDate.AddHours(-1);

        DailyBilling dailyBilling = new(100m, 0m);
        ControlledBilling controlledBilling = new(0m, 0m);
        FreeBilling freeBilling = new(0m);

        BillingPlan billingPlan = new(
            "Plano Diário",
            dailyBilling,
            controlledBilling,
            freeBilling
        );

        Vehicle vehicle = Builder<Vehicle>.CreateNew().Build();
        vehicle.FuelTankCapacity = 50;
        vehicle.SetFuelType(EFuelType.Gasoline);

        Configuration config = new(6m, 0m, 0m, 0m);

        Rental rental = new(startDate, expectedReturnDate);
        rental.AssociateBillingPlan(billingPlan);
        rental.SetBillingPlanType(EBillingPlanType.Daily);
        rental.AssociateVehicle(vehicle);
        rental.SetStartKm(0m);

        // Act
        CalculationResult result = RentalCalculator.CalculateFinalPrice(
            rental,
            actualReturnDate,
            0m,
            EFuelLevel.Full,
            config
        );

        // Assert
        Assert.AreEqual(0m, result.PenaltiesTotal);
        Assert.AreEqual(100m, result.PlanTotal);
    }

    [TestMethod]
    public void RentalCalculatorMethod_CalculateFinalPrice_WithDiscountBiggerThanTotal_ShouldNeverBelowZero()
    {
        // Arrange
        DateTimeOffset startDate = new(2025, 01, 01, 10, 00, 00, TimeSpan.Zero);
        DateTimeOffset expectedReturnDate = startDate.AddDays(1);

        DailyBilling dailyBilling = new(50m, 0m);
        ControlledBilling controlledBilling = new(0m, 0m);
        FreeBilling freeBilling = new(0m);

        BillingPlan billingPlan = new(
            "Plano Diário",
            dailyBilling,
            controlledBilling,
            freeBilling
        );

        Coupon coupon = Builder<Coupon>.CreateNew()
            .With(c => c.DiscountValue = 9999m)
            .Build();

        Vehicle vehicle = Builder<Vehicle>.CreateNew().Build();
        vehicle.FuelTankCapacity = 50;
        vehicle.SetFuelType(EFuelType.Gasoline);

        Configuration config = new(6m, 0m, 0m, 0m);

        Rental rental = new(startDate, expectedReturnDate);
        rental.AssociateBillingPlan(billingPlan);
        rental.SetBillingPlanType(EBillingPlanType.Daily);
        rental.AssociateCoupon(coupon);
        rental.AssociateVehicle(vehicle);
        rental.SetStartKm(0m);

        // Act
        CalculationResult result = RentalCalculator.CalculateFinalPrice(
            rental,
            expectedReturnDate,
            0m,
            EFuelLevel.Full,
            config
        );

        // Assert
        Assert.AreEqual(0m, result.FinalPrice);
    }

    [TestMethod]
    public void RentalCalculatorMethod_CalculateFuelPenalty_GasolineFullTank_ShouldReturnZero()
    {
        // Arrange
        Vehicle vehicle = Builder<Vehicle>.CreateNew().Build();
        vehicle.FuelTankCapacity = 50;
        vehicle.SetFuelType(EFuelType.Gasoline);

        Configuration config = new(6m, 5m, 4m, 3m);

        // Act
        decimal penalty = RentalCalculator.CalculateFuelPenalty(EFuelLevel.Full, vehicle, config);

        // Assert
        Assert.AreEqual(0m, penalty);
    }

    [TestMethod]
    public void RentalCalculatorMethod_CalculateFuelPenalty_GasMissingFuel_ShouldWorks()
    {
        // Arrange
        Vehicle vehicle = Builder<Vehicle>.CreateNew().Build();
        vehicle.FuelTankCapacity = 60;
        vehicle.SetFuelType(EFuelType.Gas);

        Configuration config = new(0m, 6m, 0m, 0m);

        // Act
        decimal penalty = RentalCalculator.CalculateFuelPenalty(EFuelLevel.Quarter, vehicle, config);

        // Assert
        decimal expectedMissingLiters = 60m * ((100 - 25) / 100m);
        decimal expectedPenalty = expectedMissingLiters * 6m;
        Assert.AreEqual(expectedPenalty, penalty);
    }

    [TestMethod]
    public void RentalCalculatorMethod_CalculateFuelPenalty_DieselMissingFuel_ShouldWorks()
    {
        // Arrange
        Vehicle vehicle = Builder<Vehicle>.CreateNew().Build();
        vehicle.FuelTankCapacity = 60;
        vehicle.SetFuelType(EFuelType.Diesel);

        Configuration config = new(0m, 0m, 7m, 0m);

        // Act
        decimal penalty = RentalCalculator.CalculateFuelPenalty(EFuelLevel.Half, vehicle, config);

        // Assert
        decimal expectedMissingLiters = 60m * ((100 - 50) / 100m);
        decimal expectedPenalty = expectedMissingLiters * 7m;
        Assert.AreEqual(expectedPenalty, penalty);
    }

    [TestMethod]
    public void RentalCalculatorMethod_CalculateFuelPenalty_AlcoholMissingFuel_ShouldWorks()
    {
        // Arrange
        Vehicle vehicle = Builder<Vehicle>.CreateNew().Build();
        vehicle.FuelTankCapacity = 60;
        vehicle.SetFuelType(EFuelType.Alcohol);

        Configuration config = new(0m, 0m, 0, 5m);

        // Act
        decimal penalty = RentalCalculator.CalculateFuelPenalty(EFuelLevel.ThreeQuarters, vehicle, config);

        // Assert
        decimal expectedMissingLiters = 60m * ((100 - 75) / 100m);
        decimal expectedPenalty = expectedMissingLiters * 5m;
        Assert.AreEqual(expectedPenalty, penalty);
    }

    [TestMethod]
    public void RentalCalculatorMethod_CalculateFuelPenalty_UnknownFuelType_ShouldReturnZero()
    {
        // Arrange
        Vehicle vehicle = Builder<Vehicle>.CreateNew().Build();
        vehicle.FuelTankCapacity = 50;
        vehicle.SetFuelType((EFuelType)999);

        Configuration config = new(10m, 10m, 10m, 10m);

        // Act
        decimal penalty = RentalCalculator.CalculateFuelPenalty(EFuelLevel.Half, vehicle, config);

        // Assert
        Assert.AreEqual(0m, penalty);
    }

    [TestMethod]
    public void RentalCalculatorMethod_CalculateFinalPrice_ShouldRoundToTwoDecimals_AwayFromZero()
    {
        // Arrange
        DateTimeOffset startDate = new(2025, 01, 01, 10, 00, 00, TimeSpan.Zero);
        DateTimeOffset expectedReturnDate = startDate.AddDays(1);

        DailyBilling dailyBilling = new(0m, 0m);
        ControlledBilling controlledBilling = new(0m, 0m);
        FreeBilling freeBilling = new(0m);

        BillingPlan billingPlan = new(
            "Plano Diário",
            dailyBilling,
            controlledBilling,
            freeBilling
        );

        Vehicle vehicle = Builder<Vehicle>.CreateNew().Build();
        vehicle.FuelTankCapacity = 1;
        vehicle.SetFuelType(EFuelType.Gasoline);

        Configuration config = new(0.005m, 0m, 0m, 0m);

        Rental rental = new(startDate, expectedReturnDate);
        rental.AssociateBillingPlan(billingPlan);
        rental.SetBillingPlanType(EBillingPlanType.Daily);
        rental.AssociateVehicle(vehicle);
        rental.SetStartKm(0m);

        // Act
        CalculationResult result = RentalCalculator.CalculateFinalPrice(
            rental,
            expectedReturnDate,
            0m,
            EFuelLevel.Empty,
            config
        );

        // Assert
        Assert.AreEqual(0.01m, result.FuelPenalty);
        Assert.AreEqual(0.01m, result.PenaltiesTotal);
        Assert.AreEqual(0.01m, result.FinalPrice);
    }
}
