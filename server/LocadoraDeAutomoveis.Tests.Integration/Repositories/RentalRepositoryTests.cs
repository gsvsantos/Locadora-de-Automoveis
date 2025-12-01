using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.PricingPlans;
using LocadoraDeAutomoveis.Domain.RateServices;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Vehicles;

namespace LocadoraDeAutomoveis.Tests.Integration.Repositories;

[TestClass]
[TestCategory("RentalRepository Infrastructure - Integration Tests")]
public sealed class RentalRepositoryTests : TestFixture
{
    [TestMethod]
    public async Task Should_GetAllAsync_ReturnsRentalsWithIncludes_Successfully()
    {
        // Arrange
        User tenant = Builder<User>.CreateNew()
            .With(t => t.FullName = "Tenant User")
            .With(t => t.UserName = $"tenantUser-{Guid.NewGuid()}")
            .With(t => t.Id = Guid.NewGuid()).Persist();
        tenant.AssociateTenant(tenant.Id);

        User userEmployee = Builder<User>.CreateNew()
            .With(uE => uE.FullName = "Employee User")
            .With(uE => uE.UserName = $"employeeUser-{Guid.NewGuid()}")
            .With(uE => uE.Id = Guid.NewGuid()).Persist();
        userEmployee.AssociateTenant(tenant.Id);

        Employee employee = Builder<Employee>.CreateNew()
            .With(e => e.FullName = userEmployee.FullName)
            .Build();

        employee.AssociateTenant(tenant.Id);
        employee.AssociateUser(userEmployee);

        await this.employeeRepository.AddAsync(employee);

        Group group = Builder<Group>.CreateNew().Build();

        group.AssociateTenant(tenant.Id);
        group.AssociateUser(userEmployee);

        await this.groupRepository.AddAsync(group);

        PricingPlan pricingPlan = Builder<PricingPlan>.CreateNew()
            .Do(v => v.DailyPlan = new(this.random.Decimal(), this.random.Decimal()))
            .Do(v => v.ControlledPlan = new(this.random.Decimal(), this.random.Int()))
            .Do(v => v.FreePlan = new(this.random.Decimal()))
            .Build();

        pricingPlan.AssociateTenant(tenant.Id);
        pricingPlan.AssociateUser(userEmployee);
        pricingPlan.AssociateGroup(group);

        await this.pricingPlanRepository.AddAsync(pricingPlan);

        Vehicle vehicle = Builder<Vehicle>.CreateNew().Build();

        vehicle.AssociateTenant(tenant.Id);
        vehicle.AssociateUser(userEmployee);
        vehicle.AssociateGroup(group);

        await this.vehicleRepository.AddAsync(vehicle);

        Client client = Builder<Client>.CreateNew()
            .Do(c => c.Address = new Address(
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.Int()
            ))
            .Do(c => c.Document = Guid.NewGuid().ToString()[..11])
            .Do(c =>
            {
                c.JuristicClientId = null;
                c.JuristicClient = null;
            })
            .Build();

        client.AssociateTenant(tenant.Id);
        client.AssociateUser(userEmployee);
        client.SetClientType(EClientType.Individual);

        await this.clientRepository.AddAsync(client);

        Driver driver = Builder<Driver>.CreateNew()
            .Do(d =>
            {
                d.Document = $"DRV-Individual-{Guid.NewGuid()}";
                d.LicenseNumber = $"LIC-Individual-{Guid.NewGuid()}";
                d.FullName = $"Driver Individual {Guid.NewGuid()}";
                d.Email = $"cpf-{Guid.NewGuid()}@test.com";
                d.LicenseValidity = DateTimeOffset.UtcNow.AddYears(2);
            })
            .Build();

        driver.AssociateTenant(tenant.Id);
        driver.AssociateUser(userEmployee);
        driver.AssociateClient(client);

        await this.driverRepository.AddAsync(driver);

        List<RateService> services = Builder<RateService>.CreateListOfSize(2)
            .Build().ToList();

        foreach (RateService service in services)
        {
            service.AssociateTenant(tenant.Id);
            service.AssociateUser(userEmployee);
        }

        await this.rateServiceRepository.AddMultiplyAsync(services);

        await this.dbContext.SaveChangesAsync();

        List<Rental> existingRentals = [];
        int quantity = 5;

        for (int i = 0; i < quantity; i++)
        {
            Rental rental = new(
                DateTimeOffset.Now.AddDays(1),
                DateTimeOffset.Now.AddDays(5),
                1000
            );

            rental.AssociateTenant(tenant.Id);
            rental.AssociateUser(userEmployee);
            rental.AssociateEmployee(employee);
            rental.AssociateClient(client);
            rental.AssociateDriver(driver);
            rental.AssociateVehicle(vehicle);
            rental.AssociatePricingPlan(pricingPlan);
            rental.AddMultiplyRateService(services);
            existingRentals.Add(rental);
        }

        await this.rentalRepository.AddMultiplyAsync(existingRentals);

        await this.dbContext.SaveChangesAsync();

        // Act
        List<Rental> rentals = await this.rentalRepository.GetAllAsync();

        // Assert
        Assert.AreEqual(quantity, rentals.Count);
        CollectionAssert.AreEquivalent(existingRentals, rentals);
    }

    [TestMethod]
    public async Task Should_GetFiveAsync_ReturnsRentalsWithIncludes_Successfully()
    {
        // Arrange
        User tenant = Builder<User>.CreateNew()
            .With(t => t.FullName = "Tenant User")
            .With(t => t.UserName = $"tenantUser-{Guid.NewGuid()}")
            .With(t => t.Id = Guid.NewGuid()).Persist();
        tenant.AssociateTenant(tenant.Id);

        User userEmployee = Builder<User>.CreateNew()
            .With(uE => uE.FullName = "Employee User")
            .With(uE => uE.UserName = $"employeeUser-{Guid.NewGuid()}")
            .With(uE => uE.Id = Guid.NewGuid()).Persist();
        userEmployee.AssociateTenant(tenant.Id);

        Employee employee = Builder<Employee>.CreateNew()
            .With(e => e.FullName = userEmployee.FullName)
            .Build();

        employee.AssociateTenant(tenant.Id);
        employee.AssociateUser(userEmployee);

        await this.dbContext.AddAsync(employee);

        Group group = Builder<Group>.CreateNew().Build();

        group.AssociateTenant(tenant.Id);
        group.AssociateUser(userEmployee);

        await this.groupRepository.AddAsync(group);

        PricingPlan pricingPlan = Builder<PricingPlan>.CreateNew()
            .Do(v => v.DailyPlan = new(this.random.Decimal(), this.random.Decimal()))
            .Do(v => v.ControlledPlan = new(this.random.Decimal(), this.random.Int()))
            .Do(v => v.FreePlan = new(this.random.Decimal()))
            .Build();

        pricingPlan.AssociateTenant(tenant.Id);
        pricingPlan.AssociateUser(userEmployee);
        pricingPlan.AssociateGroup(group);

        await this.pricingPlanRepository.AddAsync(pricingPlan);

        Vehicle vehicle = Builder<Vehicle>.CreateNew().Build();
        vehicle.AssociateTenant(tenant.Id);
        vehicle.AssociateUser(userEmployee);
        vehicle.AssociateGroup(group);

        await this.vehicleRepository.AddAsync(vehicle);

        Client client = Builder<Client>.CreateNew()
            .Do(c => c.Address = new Address(
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.Int()
            ))
            .Do(c => c.Document = Guid.NewGuid().ToString()[..11])
            .Do(c =>
            {
                c.JuristicClientId = null;
                c.JuristicClient = null;
            })
            .Build();

        client.AssociateTenant(tenant.Id);
        client.AssociateUser(userEmployee);
        client.SetClientType(EClientType.Individual);

        await this.clientRepository.AddAsync(client);

        Driver driver = Builder<Driver>.CreateNew().Build();

        driver.AssociateTenant(tenant.Id);
        driver.AssociateUser(userEmployee);
        driver.AssociateClient(client);

        await this.driverRepository.AddAsync(driver);

        await this.dbContext.SaveChangesAsync();

        List<Rental> existingRentals = [];
        for (int i = 0; i < 10; i++)
        {
            Rental rental = new(
                DateTimeOffset.Now,
                DateTimeOffset.Now.AddDays(5),
                1000
            );

            rental.AssociateTenant(tenant.Id);
            rental.AssociateUser(userEmployee);
            rental.AssociateEmployee(employee);
            rental.AssociateClient(client);
            rental.AssociateDriver(driver);
            rental.AssociateVehicle(vehicle);
            rental.AssociatePricingPlan(pricingPlan);
            existingRentals.Add(rental);
        }

        await this.rentalRepository.AddMultiplyAsync(existingRentals);

        await this.dbContext.SaveChangesAsync();

        // Act 
        List<Rental> rentals = await this.rentalRepository.GetAllAsync(5);

        // Assert
        Assert.AreEqual(5, rentals.Count);
        for (int i = 0; i < 5; i++)
        {
            Assert.IsNotNull(rentals[i].Client);
            Assert.IsNotNull(rentals[i].Vehicle);
        }
    }

    [TestMethod]
    public async Task Should_GetRentalById_ReturnsRentalWithIncludes_Successfully()
    {
        // Arrange
        User tenant = Builder<User>.CreateNew()
            .With(t => t.FullName = "Tenant User")
            .With(t => t.UserName = $"tenantUser-{Guid.NewGuid()}")
            .With(t => t.Id = Guid.NewGuid()).Persist();
        tenant.AssociateTenant(tenant.Id);

        User userEmployee = Builder<User>.CreateNew()
            .With(uE => uE.FullName = "Employee User")
            .With(uE => uE.UserName = $"employeeUser-{Guid.NewGuid()}")
            .With(uE => uE.Id = Guid.NewGuid()).Persist();
        userEmployee.AssociateTenant(tenant.Id);

        Employee employee = Builder<Employee>.CreateNew()
            .With(e => e.FullName = userEmployee.FullName)
            .Build();

        employee.AssociateTenant(tenant.Id);
        employee.AssociateUser(userEmployee);

        await this.dbContext.AddAsync(employee);

        Group group = Builder<Group>.CreateNew().Build();

        group.AssociateTenant(tenant.Id);
        group.AssociateUser(userEmployee);

        await this.groupRepository.AddAsync(group);

        PricingPlan pricingPlan = Builder<PricingPlan>.CreateNew()
            .Do(v => v.DailyPlan = new(this.random.Decimal(), this.random.Decimal()))
            .Do(v => v.ControlledPlan = new(this.random.Decimal(), this.random.Int()))
            .Do(v => v.FreePlan = new(this.random.Decimal()))
            .Build();

        pricingPlan.AssociateTenant(tenant.Id);
        pricingPlan.AssociateUser(userEmployee);
        pricingPlan.AssociateGroup(group);

        await this.pricingPlanRepository.AddAsync(pricingPlan);

        Vehicle vehicle = Builder<Vehicle>.CreateNew().Build();

        vehicle.AssociateTenant(tenant.Id);
        vehicle.AssociateUser(userEmployee);
        vehicle.AssociateGroup(group);

        await this.vehicleRepository.AddAsync(vehicle);

        Client client = Builder<Client>.CreateNew()
            .Do(c => c.Address = new Address(
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.NextString(5, 5),
                this.random.Int()
            ))
            .Do(c => c.Document = Guid.NewGuid().ToString()[..11])
            .Do(c =>
            {
                c.JuristicClientId = null;
                c.JuristicClient = null;
            })
            .Build();

        client.AssociateTenant(tenant.Id);
        client.AssociateUser(userEmployee);
        client.SetClientType(EClientType.Individual);

        await this.clientRepository.AddAsync(client);

        Driver driver = Builder<Driver>.CreateNew().Build();

        driver.AssociateTenant(tenant.Id);
        driver.AssociateUser(userEmployee);
        driver.AssociateClient(client);

        await this.driverRepository.AddAsync(driver);

        await this.dbContext.SaveChangesAsync();

        Rental rental = new(
            DateTimeOffset.Now,
            DateTimeOffset.Now.AddDays(3),
            1000
        );

        rental.AssociateTenant(tenant.Id);
        rental.AssociateUser(userEmployee);
        rental.AssociateEmployee(employee);
        rental.AssociateClient(client);
        rental.AssociateDriver(driver);
        rental.AssociateVehicle(vehicle);
        rental.AssociatePricingPlan(pricingPlan);

        await this.rentalRepository.AddAsync(rental);

        await this.dbContext.SaveChangesAsync();

        // Act
        Rental? selectedRental = await this.rentalRepository.GetByIdAsync(rental.Id);

        // Assert
        Assert.IsNotNull(selectedRental);
        Assert.AreEqual(rental.Id, selectedRental.Id);
        Assert.AreEqual(rental.Client.Id, selectedRental.Client.Id);
        Assert.AreEqual(rental.Vehicle.Id, selectedRental.Vehicle.Id);
        Assert.IsNotNull(selectedRental.Vehicle.Group);
        Assert.AreEqual(rental.Vehicle.Group.Id, selectedRental.Vehicle.Group.Id);
        Assert.AreEqual(rental.Driver.Id, selectedRental.Driver.Id);
        Assert.AreEqual(rental.PricingPlan.Id, selectedRental.PricingPlan.Id);
        Assert.IsNotNull(rental.Employee);
        Assert.IsNotNull(selectedRental.Employee);
        Assert.AreEqual(rental.Employee.Id, selectedRental.Employee.Id);
    }
}