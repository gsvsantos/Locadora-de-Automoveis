using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.PricingPlans;
using LocadoraDeAutomoveis.Domain.RateServices;
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
        Assert.IsNotNull(rental.RateServices);
        Assert.AreEqual(0, rental.RateServices.Count);
    }

    [TestMethod]
    public void RentalConstructor_Parametered_ShouldInitializeProperties()
    {
        // Arrange
        DateTimeOffset start = DateTimeOffset.Now;
        DateTimeOffset end = start.AddDays(5);
        decimal startKm = 15000;

        // Act
        Rental rental = new(start, end, startKm);

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
    public void RentalMethod_AssociatePricingPlan_ShouldWorks()
    {
        // Arrange
        Rental rental = new();
        PricingPlan plan = Builder<PricingPlan>.CreateNew().Build();

        // Act
        rental.AssociatePricingPlan(plan);

        // Assert
        Assert.AreEqual(plan.Id, rental.PricingPlanId);
        Assert.AreEqual(plan, rental.PricingPlan);
    }

    [TestMethod]
    public void RentalMethod_CalculateBasePrice_DailyPlan_ShouldSumDailyRateAndKmPrice()
    {
        // Arrange
        Rental rental = new();
        PricingPlan plan = new(
            "Plano Teste",
            new DailyPlanProps(100, 10),
            new ControlledPlanProps(0, 0),
            new FreePlanProps(0)
        );

        rental.AssociatePricingPlan(plan);
        rental.SetPricingPlanType(EPricingPlanType.Daily);

        // Act
        rental.CalculateBasePrice();

        // Assert
        Assert.AreEqual(110, rental.BaseRentalPrice);
    }

    [TestMethod]
    public void RentalMethod_CalculateBasePrice_ControlledPlan_ShouldSumDailyRateAndExtrapolatedKmPrice()
    {
        // Arrange
        Rental rental = new();
        PricingPlan plan = new(
            "Plano Teste",
            new DailyPlanProps(0, 0),
            new ControlledPlanProps(80, 20),
            new FreePlanProps(0)
        );

        rental.AssociatePricingPlan(plan);
        rental.SetPricingPlanType(EPricingPlanType.Controlled);

        // Act
        rental.CalculateBasePrice();

        // Assert
        Assert.AreEqual(100, rental.BaseRentalPrice);
    }

    [TestMethod]
    public void RentalMethod_CalculateBasePrice_FreePlan_ShouldUseFixedRate()
    {
        // Arrange
        Rental rental = new();
        PricingPlan plan = new(
            "Plano Teste",
            new DailyPlanProps(0, 0),
            new ControlledPlanProps(0, 0),
            new FreePlanProps(250)
        );

        rental.AssociatePricingPlan(plan);
        rental.SetPricingPlanType(EPricingPlanType.Free);

        // Act
        rental.CalculateBasePrice();

        // Assert
        Assert.AreEqual(250, rental.BaseRentalPrice);
    }

    [TestMethod]
    public void RentalMethod_AddRentalService_ShouldAddToList()
    {
        // Arrange
        Rental rental = new();
        RateService service = Builder<RateService>.CreateNew().Build();

        // Act
        rental.AddRentalService(service);

        // Assert
        Assert.AreEqual(1, rental.RateServices.Count);
        Assert.IsTrue(rental.RateServices.Contains(service));
    }

    [TestMethod]
    public void RentalMethod_AddMultiplyRateService_ShouldAddRangeToList()
    {
        // Arrange
        Rental rental = new();
        List<RateService> services = Builder<RateService>.CreateListOfSize(3).Build().ToList();

        // Act
        rental.AddRangeRateServices(services);

        // Assert
        Assert.AreEqual(3, rental.RateServices.Count);
    }

    [TestMethod]
    public void RentalMethod_RemoveRentalService_ShouldRemoveFromList()
    {
        // Arrange
        Rental rental = new();
        RateService service = Builder<RateService>.CreateNew().Build();
        rental.AddRentalService(service);

        // Act
        rental.RemoveRentalService(service);

        // Assert
        Assert.AreEqual(0, rental.RateServices.Count);
    }

    [TestMethod]
    public void RentalMethod_Setters_ShouldUpdateValues()
    {
        // Arrange
        Rental rental = new();

        // Act
        rental.SetEstimatedKilometers(500);
        rental.SetStatus(ERentalStatus.Completed);
        rental.SetFinalPrice(1200);

        // Assert
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
            DateTimeOffset.Now.AddDays(1),
            100
        );

        rental.AssociateClient(Builder<Client>.CreateNew().Build());
        rental.AssociateDriver(Builder<Driver>.CreateNew().Build());
        rental.AssociatePricingPlan(Builder<PricingPlan>.CreateNew().Build());

        Rental updatedData = new(
            DateTimeOffset.Now.AddDays(2),
            DateTimeOffset.Now.AddDays(7),
            200
        );

        Employee newEmployee = Builder<Employee>.CreateNew().Build();
        Client newClient = Builder<Client>.CreateNew().Build();
        Driver newDriver = Builder<Driver>.CreateNew().Build();
        PricingPlan newPlan = Builder<PricingPlan>.CreateNew().Build();
        List<RateService> newServices = Builder<RateService>.CreateListOfSize(2).Build().ToList();

        updatedData.AssociateEmployee(newEmployee);
        updatedData.AssociateClient(newClient);
        updatedData.AssociateDriver(newDriver);
        updatedData.AssociatePricingPlan(newPlan);
        updatedData.AddRangeRateServices(newServices);

        // Act
        rental.Update(updatedData);

        // Assert
        Assert.AreEqual(updatedData.StartDate, rental.StartDate);
        Assert.AreEqual(updatedData.ExpectedReturnDate, rental.ExpectedReturnDate);
        Assert.AreEqual(updatedData.StartKm, rental.StartKm);
        Assert.AreEqual(newEmployee.Id, rental.EmployeeId);
        Assert.AreEqual(newClient.Id, rental.ClientId);
        Assert.AreEqual(newDriver.Id, rental.DriverId);
        Assert.AreEqual(newPlan.Id, rental.PricingPlanId);
        Assert.AreEqual(2, rental.RateServices.Count);
    }
}