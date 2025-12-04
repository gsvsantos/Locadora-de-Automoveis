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
[TestCategory("RentalReturnRepository Infrastructure - Integration Tests")]
public sealed class RentalReturnRepositoryTests : TestFixture
{
    [TestMethod]
    public async Task Should_GetAllAsync_ReturnsRentalReturnsWithIncludes_Successfully()
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
            .With(e => e.AdmissionDate = DateTimeOffset.Now)
            .Build();

        employee.AssociateTenant(tenant.Id);
        employee.AssociateUser(userEmployee);

        await this.dbContext.AddAsync(employee);

        Group group = Builder<Group>.CreateNew().Build();

        group.AssociateTenant(tenant.Id);
        group.AssociateUser(userEmployee);

        await this.groupRepository.AddAsync(group);

        RandomGenerator random = new();
        BillingPlan billingPlan = Builder<BillingPlan>.CreateNew()
            .Do(v => v.Daily = new(random.Decimal(), random.Decimal()))
            .Do(v => v.Controlled = new(random.Decimal(), random.Int()))
            .Do(v => v.Free = new(random.Decimal()))
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

        List<RentalReturn> existingRentals = [];
        int quantity = 5;

        for (int i = 0; i < quantity; i++)
        {
            Rental rental = new(
                DateTimeOffset.Now.AddDays(-5),
                DateTimeOffset.Now.AddDays(1),
                1000
            );

            rental.AssociateTenant(tenant.Id);
            rental.AssociateUser(userEmployee);
            rental.AssociateEmployee(employee);
            rental.AssociateClient(client);
            rental.AssociateDriver(driver);
            rental.AssociateVehicle(vehicle);
            rental.AssociateBillingPlan(billingPlan);
            rental.AddRangeExtras(extras);

            await this.rentalRepository.AddAsync(rental);

            RentalReturn rentalReturn = new(
                DateTimeOffset.Now,
                1200,
                200
            );

            rentalReturn.AssociateTenant(tenant.Id);
            rentalReturn.AssociateUser(userEmployee);
            rentalReturn.AssociateRental(rental);
            existingRentals.Add(rentalReturn);
        }

        await this.rentalReturnRepository.AddMultiplyAsync(existingRentals);

        await this.dbContext.SaveChangesAsync();

        // Act
        List<RentalReturn> rentals = await this.rentalReturnRepository.GetAllAsync();

        // Assert
        Assert.AreEqual(quantity, rentals.Count);
        CollectionAssert.AreEquivalent(existingRentals, rentals);
    }

    [TestMethod]
    public async Task Should_GetFiveAsync_ReturnsRentalReturnsWithIncludes_Successfully()
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

        List<RentalExtra> extras = Builder<RentalExtra>.CreateListOfSize(2)
            .Build().ToList();

        foreach (RentalExtra extra in extras)
        {
            extra.AssociateTenant(tenant.Id);
            extra.AssociateUser(userEmployee);
        }

        await this.rentalExtraRepository.AddMultiplyAsync(extras);

        await this.dbContext.SaveChangesAsync();

        List<RentalReturn> existingReturns = [];
        for (int i = 0; i < 10; i++)
        {
            Rental rental = new(
                DateTimeOffset.Now.AddDays(-10),
                DateTimeOffset.Now.AddDays(-5),
                1000
            );

            rental.AssociateTenant(tenant.Id);
            rental.AssociateUser(userEmployee);
            rental.AssociateEmployee(employee);
            rental.AssociateClient(client);
            rental.AssociateDriver(driver);
            rental.AssociateVehicle(vehicle);
            rental.AssociateBillingPlan(BillingPlan);
            rental.AddRangeExtras(extras);

            await this.rentalRepository.AddAsync(rental);

            RentalReturn rentalReturn = new(
                DateTimeOffset.Now,
                1500,
                500
            );

            rentalReturn.AssociateTenant(tenant.Id);
            rentalReturn.AssociateUser(userEmployee);
            rentalReturn.AssociateRental(rental);

            existingReturns.Add(rentalReturn);
        }

        await this.rentalReturnRepository.AddMultiplyAsync(existingReturns);

        await this.dbContext.SaveChangesAsync();

        // Act 
        List<RentalReturn> returns = await this.rentalReturnRepository.GetAllAsync(5);

        // Assert
        Assert.AreEqual(5, returns.Count);
        for (int i = 0; i < 5; i++)
        {
            Assert.IsNotNull(returns[i].Rental);
            Assert.IsNotNull(returns[i].Rental.Client);
            Assert.IsNotNull(returns[i].Rental.Vehicle);
            Assert.IsNotNull(returns[i].Rental.Vehicle.Group);
        }
    }

    [TestMethod]
    public async Task Should_GetRentalReturnById_ReturnsRentalReturnWithIncludes_Successfully()
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
            DateTimeOffset.Now.AddDays(3),
            1000
        );

        rental.AssociateTenant(tenant.Id);
        rental.AssociateUser(userEmployee);
        rental.AssociateEmployee(employee);
        rental.AssociateClient(client);
        rental.AssociateDriver(driver);
        rental.AssociateVehicle(vehicle);
        rental.AssociateBillingPlan(BillingPlan);

        await this.rentalRepository.AddAsync(rental);

        RentalReturn rentalReturn = new(
            DateTimeOffset.Now,
            1200,
            200
        );

        rentalReturn.AssociateTenant(tenant.Id);
        rentalReturn.AssociateUser(userEmployee);
        rentalReturn.AssociateRental(rental);

        await this.rentalReturnRepository.AddAsync(rentalReturn);

        await this.dbContext.SaveChangesAsync();

        // Act
        RentalReturn? selectedReturn = await this.rentalReturnRepository.GetByIdAsync(rentalReturn.Id);

        // Assert
        Assert.IsNotNull(selectedReturn);
        Assert.AreEqual(rentalReturn.Id, selectedReturn.Id);
        Assert.AreEqual(rental.Id, selectedReturn.Rental.Id);
        Assert.AreEqual(vehicle.LicensePlate, selectedReturn.Rental.Vehicle.LicensePlate);
        Assert.AreEqual(group.Id, selectedReturn.Rental.Vehicle.Group.Id);
        Assert.AreEqual(employee.Id, selectedReturn.Rental.Employee!.Id);
    }
}