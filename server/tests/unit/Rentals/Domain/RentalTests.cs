using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Vehicles;

namespace LocadoraDeAutomoveis.Tests.Unit.Rentals.Domain;

[TestClass]
[TestCategory("Rental Domain - Unit Tests")]
public sealed class RentalTests
{
    [TestMethod]
    public void RentalConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        Rental rental = new();

        // Assert
        Assert.AreNotEqual(Guid.Empty, rental.Id);
        Assert.IsTrue(rental.IsActive);
        Assert.AreEqual(0, rental.StartKm);
        Assert.AreEqual(0, rental.BaseRentalPrice);
        Assert.AreEqual(1000, rental.GuaranteeValue);
        Assert.AreEqual(0, rental.FinalPrice);
        Assert.IsNotNull(rental.Extras);
        Assert.AreEqual(0, rental.Extras.Count);
    }

    [TestMethod]
    public void RentalConstructor_Parametered_ShouldWorks()
    {
        // Arrange
        DateTimeOffset start = DateTimeOffset.Now;
        DateTimeOffset end = start.AddDays(5);
        decimal startKm = 15000;

        // Act
        Rental rental = new(start, end);
        rental.SetStartKm(startKm);

        // Assert
        Assert.AreEqual(start, rental.StartDate);
        Assert.AreEqual(end, rental.ExpectedReturnDate);
        Assert.AreEqual(startKm, rental.StartKm);
        Assert.AreEqual(1000, rental.GuaranteeValue);
    }

    [TestMethod]
    public void RentalMethod_AssociateClient_ShouldWorks()
    {
        // Arrange
        Rental rental = new();
        Client client = Builder<Client>.CreateNew().Build();

        // Act
        rental.AssociateClient(client);

        // Assert
        Assert.AreEqual(client.Id, rental.ClientId);
        Assert.AreEqual(client, rental.Client);
    }

    [TestMethod]
    public void RentalMethod_AssociateDriver_ShouldWorks()
    {
        // Arrange
        Rental rental = new();
        Driver driver = Builder<Driver>.CreateNew().Build();

        // Act
        rental.AssociateDriver(driver);

        // Assert
        Assert.AreEqual(driver.Id, rental.DriverId);
        Assert.AreEqual(driver, rental.Driver);
    }

    [TestMethod]
    public void RentalMethod_AssociateVehicle_ShouldWorks()
    {
        // Arrange
        Rental rental = new();
        Vehicle vehicle = Builder<Vehicle>.CreateNew().Build();

        // Act
        rental.AssociateVehicle(vehicle);

        // Assert
        Assert.AreEqual(vehicle.Id, rental.VehicleId);
        Assert.AreEqual(vehicle, rental.Vehicle);
    }

    [TestMethod]
    public void RentalMethod_AssociateEmployee_ShouldWorks()
    {
        // Arrange
        Rental rental = new();
        Employee employee = Builder<Employee>.CreateNew().Build();

        // Act
        rental.AssociateEmployee(employee);

        // Assert
        Assert.AreEqual(employee.Id, rental.EmployeeId);
        Assert.AreEqual(employee, rental.Employee);
    }

    [TestMethod]
    public void RentalMethod_AssociateCoupon_ShouldWorks()
    {
        // Arrange
        Rental rental = new();
        Coupon coupon = Builder<Coupon>.CreateNew().Build();

        // Act
        rental.AssociateCoupon(coupon);

        // Assert
        Assert.AreEqual(coupon.Id, rental.CouponId);
        Assert.AreEqual(coupon, rental.Coupon);
    }

    [TestMethod]
    public void RentalMethod_AssociateBillingPlan_ShouldWorks()
    {
        // Arrange
        Rental rental = new();
        BillingPlan plan = Builder<BillingPlan>.CreateNew().Build();

        // Act
        rental.AssociateBillingPlan(plan);

        // Assert
        Assert.AreEqual(plan.Id, rental.BillingPlanId);
        Assert.AreEqual(plan, rental.BillingPlan);
    }

    [TestMethod]
    public void RentalMethod_AddExtra_ShouldAddToList()
    {
        // Arrange
        Rental rental = new();
        RentalExtra extra = Builder<RentalExtra>.CreateNew().Build();

        // Act
        rental.AddExtra(extra);

        // Assert
        Assert.AreEqual(1, rental.Extras.Count);
        Assert.IsTrue(rental.Extras.Contains(extra));
    }

    [TestMethod]
    public void RentalMethod_AddRangeExtras_ShouldAddRangeToList()
    {
        // Arrange
        Rental rental = new();
        List<RentalExtra> extras = Builder<RentalExtra>.CreateListOfSize(3).Build().ToList();

        // Act
        rental.AddRangeExtras(extras);

        // Assert
        Assert.AreEqual(3, rental.Extras.Count);
    }

    [TestMethod]
    public void RentalMethod_RemoveExtra_ShouldRemoveFromList()
    {
        // Arrange
        Rental rental = new();
        RentalExtra extra = Builder<RentalExtra>.CreateNew().Build();
        rental.AddExtra(extra);

        // Act
        rental.RemoveExtra(extra);

        // Assert
        Assert.AreEqual(0, rental.Extras.Count);
    }

    [TestMethod]
    public void RentalMethod_Setters_ShouldUpdateValues()
    {
        // Arrange
        Rental rental = new();

        // Act
        rental.SetBasePrice(1500);
        rental.SetEstimatedKilometers(500);
        rental.SetStatus(ERentalStatus.Completed);
        rental.SetFinalPrice(1200);

        // Assert
        Assert.AreEqual(1500, rental.BaseRentalPrice);
        Assert.AreEqual(500, rental.EstimatedKilometers);
        Assert.AreEqual(ERentalStatus.Completed, rental.Status);
        Assert.AreEqual(1200, rental.FinalPrice);
    }

    [TestMethod]
    public void RentalMethod_Update_ShouldWorks()
    {
        // Arrange
        Rental rental = new(
            DateTimeOffset.Now,
            DateTimeOffset.Now.AddDays(1)
        );

        rental.AssociateClient(Builder<Client>.CreateNew().Build());
        rental.AssociateDriver(Builder<Driver>.CreateNew().Build());
        rental.AssociateBillingPlan(Builder<BillingPlan>.CreateNew().Build());
        rental.SetStartKm(100);

        Rental updatedData = new(
            DateTimeOffset.Now.AddDays(2),
            DateTimeOffset.Now.AddDays(7)
        );
        updatedData.SetStartKm(200);

        Employee newEmployee = Builder<Employee>.CreateNew().Build();
        Coupon newCoupon = Builder<Coupon>.CreateNew().Build();
        Client newClient = Builder<Client>.CreateNew().Build();
        Driver newDriver = Builder<Driver>.CreateNew().Build();
        Vehicle newVehicle = Builder<Vehicle>.CreateNew().Build();
        BillingPlan newPlan = Builder<BillingPlan>.CreateNew().Build();
        List<RentalExtra> newExtras = Builder<RentalExtra>.CreateListOfSize(2).Build().ToList();

        updatedData.AssociateEmployee(newEmployee);
        updatedData.AssociateCoupon(newCoupon);
        updatedData.AssociateClient(newClient);
        updatedData.AssociateDriver(newDriver);
        updatedData.AssociateVehicle(newVehicle);
        updatedData.AssociateBillingPlan(newPlan);
        updatedData.AddRangeExtras(newExtras);
        updatedData.SetEstimatedKilometers(300);

        // Act
        rental.Update(updatedData);

        // Assert
        Assert.AreEqual(updatedData.StartDate, rental.StartDate);
        Assert.AreEqual(updatedData.ExpectedReturnDate, rental.ExpectedReturnDate);
        Assert.AreEqual(updatedData.StartKm, rental.StartKm);
        Assert.AreEqual(newEmployee.Id, rental.EmployeeId);
        Assert.AreEqual(newCoupon.Id, rental.CouponId);
        Assert.AreEqual(newClient.Id, rental.ClientId);
        Assert.AreEqual(newDriver.Id, rental.DriverId);
        Assert.AreEqual(newPlan.Id, rental.BillingPlanId);
        Assert.AreEqual(2, rental.Extras.Count);
        Assert.AreEqual(300, rental.EstimatedKilometers);
    }
}