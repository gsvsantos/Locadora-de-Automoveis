using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Employees;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.RentalExtras;
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
            .Do(e =>
            {
                e.LoginUserId = null;
                e.LoginUser = null;
            })
            .Build();

        employee.AssociateTenant(tenant.Id);
        employee.AssociateUser(userEmployee);

        await this.employeeRepository.AddAsync(employee);

        Group group = Builder<Group>.CreateNew().Build();

        group.AssociateTenant(tenant.Id);
        group.AssociateUser(userEmployee);

        await this.groupRepository.AddAsync(group);

        BillingPlan billingPlan = Builder<BillingPlan>.CreateNew()
            .Do(v => v.Daily = new(this.random.Decimal(), this.random.Decimal()))
            .Do(v => v.Controlled = new(this.random.Decimal(), this.random.Int()))
            .Do(v => v.Free = new(this.random.Decimal()))
            .Build();

        billingPlan.AssociateTenant(tenant.Id);
        billingPlan.AssociateUser(userEmployee);
        billingPlan.AssociateGroup(group);

        await this.BillingPlanRepository.AddAsync(billingPlan);

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
                c.LoginUserId = null;
                c.LoginUser = null;
            })
            .Build();

        client.AssociateTenant(tenant.Id);
        client.AssociateUser(userEmployee);
        client.DefineType(EClientType.Individual);

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

        List<RentalExtra> extras = Builder<RentalExtra>.CreateListOfSize(2)
            .Build().ToList();

        foreach (RentalExtra extra in extras)
        {
            extra.AssociateTenant(tenant.Id);
            extra.AssociateUser(userEmployee);
        }

        await this.rentalExtraRepository.AddMultiplyAsync(extras);

        await this.dbContext.SaveChangesAsync();

        List<Rental> existingRentals = [];
        int quantity = 5;

        for (int i = 0; i < quantity; i++)
        {
            Rental rental = new(
                DateTimeOffset.Now.AddDays(1),
                DateTimeOffset.Now.AddDays(5)
            );

            rental.AssociateTenant(tenant.Id);
            rental.AssociateUser(userEmployee);
            rental.AssociateEmployee(employee);
            rental.AssociateClient(client);
            rental.AssociateDriver(driver);
            rental.AssociateVehicle(vehicle);
            rental.AssociateBillingPlan(billingPlan);
            rental.AddRangeExtras(extras);
            rental.SetStartKm(vehicle.Kilometers);
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
            .Do(e =>
            {
                e.LoginUserId = null;
                e.LoginUser = null;
            })
            .Build();

        employee.AssociateTenant(tenant.Id);
        employee.AssociateUser(userEmployee);

        await this.dbContext.AddAsync(employee);

        Group group = Builder<Group>.CreateNew().Build();

        group.AssociateTenant(tenant.Id);
        group.AssociateUser(userEmployee);

        await this.groupRepository.AddAsync(group);

        BillingPlan BillingPlan = Builder<BillingPlan>.CreateNew()
            .Do(v => v.Daily = new(this.random.Decimal(), this.random.Decimal()))
            .Do(v => v.Controlled = new(this.random.Decimal(), this.random.Int()))
            .Do(v => v.Free = new(this.random.Decimal()))
            .Build();

        BillingPlan.AssociateTenant(tenant.Id);
        BillingPlan.AssociateUser(userEmployee);
        BillingPlan.AssociateGroup(group);

        await this.BillingPlanRepository.AddAsync(BillingPlan);

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
                c.LoginUserId = null;
                c.LoginUser = null;
            })
            .Build();

        client.AssociateTenant(tenant.Id);
        client.AssociateUser(userEmployee);
        client.DefineType(EClientType.Individual);

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
                DateTimeOffset.Now.AddDays(5)
            );

            rental.AssociateTenant(tenant.Id);
            rental.AssociateUser(userEmployee);
            rental.AssociateEmployee(employee);
            rental.AssociateClient(client);
            rental.AssociateDriver(driver);
            rental.AssociateVehicle(vehicle);
            rental.AssociateBillingPlan(BillingPlan);
            rental.SetStartKm(vehicle.Kilometers);
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
            .Do(e =>
            {
                e.LoginUserId = null;
                e.LoginUser = null;
            })
            .Build();

        employee.AssociateTenant(tenant.Id);
        employee.AssociateUser(userEmployee);

        await this.dbContext.AddAsync(employee);

        Group group = Builder<Group>.CreateNew().Build();

        group.AssociateTenant(tenant.Id);
        group.AssociateUser(userEmployee);

        await this.groupRepository.AddAsync(group);

        BillingPlan BillingPlan = Builder<BillingPlan>.CreateNew()
            .Do(v => v.Daily = new(this.random.Decimal(), this.random.Decimal()))
            .Do(v => v.Controlled = new(this.random.Decimal(), this.random.Int()))
            .Do(v => v.Free = new(this.random.Decimal()))
            .Build();

        BillingPlan.AssociateTenant(tenant.Id);
        BillingPlan.AssociateUser(userEmployee);
        BillingPlan.AssociateGroup(group);

        await this.BillingPlanRepository.AddAsync(BillingPlan);

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
                c.LoginUserId = null;
                c.LoginUser = null;
            })
            .Build();

        client.AssociateTenant(tenant.Id);
        client.AssociateUser(userEmployee);
        client.DefineType(EClientType.Individual);

        await this.clientRepository.AddAsync(client);

        Driver driver = Builder<Driver>.CreateNew().Build();

        driver.AssociateTenant(tenant.Id);
        driver.AssociateUser(userEmployee);
        driver.AssociateClient(client);

        await this.driverRepository.AddAsync(driver);

        await this.dbContext.SaveChangesAsync();

        Rental rental = new(
            DateTimeOffset.Now,
            DateTimeOffset.Now.AddDays(3)
        );

        rental.AssociateTenant(tenant.Id);
        rental.AssociateUser(userEmployee);
        rental.AssociateEmployee(employee);
        rental.AssociateClient(client);
        rental.AssociateDriver(driver);
        rental.AssociateVehicle(vehicle);
        rental.AssociateBillingPlan(BillingPlan);
        rental.SetStartKm(vehicle.Kilometers);

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
        Assert.AreEqual(rental.BillingPlan.Id, selectedRental.BillingPlan.Id);
        Assert.IsNotNull(rental.Employee);
        Assert.IsNotNull(selectedRental.Employee);
        Assert.AreEqual(rental.Employee.Id, selectedRental.Employee.Id);
    }
}