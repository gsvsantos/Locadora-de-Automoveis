using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Vehicles;

namespace LocadoraDeAutomoveis.Tests.Integration.Repositories;

[TestClass]
[TestCategory("VehicleRepository Infrastructure - Integration Tests")]
public sealed class VehicleRepositoryTests : TestFixture
{
    [TestMethod]
    public async Task Should_GetAllAsync_ReturnsVehicleWithGroup_Successfully()
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

        List<Group> existingGroups = Builder<Group>.CreateListOfSize(2).Build().ToList();
        foreach (Group group in existingGroups)
        {
            group.AssociateTenant(tenant.Id);
            group.AssociateUser(userEmployee);
        }

        await this.groupRepository.AddMultiplyAsync(existingGroups);

        await this.dbContext.SaveChangesAsync();

        List<Vehicle> existingVehicles = Builder<Vehicle>.CreateListOfSize(10).Build().ToList();
        foreach (Vehicle vehicle in existingVehicles)
        {
            vehicle.AssociateTenant(tenant.Id);
            vehicle.AssociateUser(userEmployee);
            vehicle.AssociateGroup(existingGroups[0]);
        }

        existingVehicles[4].AssociateGroup(existingGroups[1]);
        existingVehicles[8].AssociateGroup(existingGroups[1]);

        await this.vehicleRepository.AddMultiplyAsync(existingVehicles);

        await this.dbContext.SaveChangesAsync();

        // Act 
        List<Vehicle> vehicles = await this.vehicleRepository.GetAllAsync();

        // Assert
        CollectionAssert.AreEquivalent(existingVehicles, vehicles);
        Assert.AreEqual(10, vehicles.Count);
        Assert.AreEqual(existingGroups[0], existingVehicles[2].Group);
        Assert.AreEqual(existingGroups[1], existingVehicles[4].Group);
        Assert.AreEqual(existingGroups[0], existingVehicles[6].Group);
        Assert.AreEqual(existingGroups[1], existingVehicles[8].Group);
    }

    [TestMethod]
    public async Task Should_GetFiveAsync_ReturnsVehicleWithGroup_Successfully()
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

        List<Group> existingGroups = Builder<Group>.CreateListOfSize(2).Build().ToList();
        foreach (Group group in existingGroups)
        {
            group.AssociateTenant(tenant.Id);
            group.AssociateUser(userEmployee);
        }

        await this.groupRepository.AddMultiplyAsync(existingGroups);

        await this.dbContext.SaveChangesAsync();

        List<Vehicle> existingVehicles = Builder<Vehicle>.CreateListOfSize(10).Build().ToList();
        foreach (Vehicle vehicle in existingVehicles)
        {
            vehicle.AssociateTenant(tenant.Id);
            vehicle.AssociateUser(userEmployee);
            vehicle.AssociateGroup(existingGroups[0]);
        }

        existingVehicles[1].AssociateGroup(existingGroups[1]);
        existingVehicles[4].AssociateGroup(existingGroups[1]);

        await this.vehicleRepository.AddMultiplyAsync(existingVehicles);

        await this.dbContext.SaveChangesAsync();

        // Act 
        List<Vehicle> vehicles = await this.vehicleRepository.GetAllAsync(5);

        // Assert
        Assert.AreEqual(5, vehicles.Count);
        Assert.AreEqual(existingGroups[1], existingVehicles[1].Group);
        Assert.AreEqual(existingGroups[0], existingVehicles[2].Group);
        Assert.AreEqual(existingGroups[0], existingVehicles[3].Group);
        Assert.AreEqual(existingGroups[1], existingVehicles[4].Group);
        Assert.AreEqual(existingGroups[0], existingVehicles[5].Group);
    }

    [TestMethod]
    public async Task Should_GetVehicleByIdAsync_ReturnsVehicleWithGroup_Successfully()
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

        Group group = Builder<Group>.CreateNew().Build();
        group.AssociateTenant(tenant.Id);
        group.AssociateUser(userEmployee);

        Vehicle vehicle = Builder<Vehicle>.CreateNew().Build();
        vehicle.AssociateTenant(tenant.Id);
        vehicle.AssociateUser(userEmployee);
        vehicle.AssociateGroup(group);

        await this.vehicleRepository.AddAsync(vehicle);

        await this.dbContext.CommitAsync();

        // Act 
        Vehicle? selectedVehicles = await this.vehicleRepository.GetByIdAsync(vehicle.Id);

        // Assert
        Assert.IsNotNull(selectedVehicles);
        Assert.AreEqual(group, selectedVehicles.Group);
    }
}
