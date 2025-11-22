using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Vehicles;
using LocadoraDeAutomoveis.Tests.Integration.Shared;

namespace LocadoraDeAutomoveis.Tests.Integration.Repositories;

[TestClass]
[TestCategory("GroupRepository Infrastructure - Integration Tests")]
public sealed class GroupRepositoryTests : TestFixture
{
    [TestMethod]
    public async Task Should_GetAllAsync_ReturnGroupsWithVehicles_Successfuly()
    {
        // Arrange
        User tenant = Builder<User>.CreateNew()
            .With(t => t.FullName == "Tenant User")
            .With(t => t.UserName = $"tenantUser-{Guid.NewGuid()}")
            .With(t => t.Id == Guid.NewGuid()).Persist();
        tenant.AssociateTenant(tenant.Id);

        User userEmployee = Builder<User>.CreateNew()
            .With(uE => uE.FullName == "Employee User")
            .With(uE => uE.UserName = $"employeeUser-{Guid.NewGuid()}")
            .With(uE => uE.Id == Guid.NewGuid()).Persist();
        userEmployee.AssociateTenant(tenant.Id);

        List<Group> existingGroups = Builder<Group>.CreateListOfSize(10).Build().ToList();
        foreach (Group group in existingGroups)
        {
            group.AssociateTenant(tenant.Id);
            group.AssociateUser(userEmployee);
        }

        Vehicle vehicle = Builder<Vehicle>.CreateNew().Build();
        vehicle.AssociateTenant(tenant.Id);
        vehicle.AssociateUser(userEmployee);

        existingGroups[3].AddVehicle(vehicle);

        await this.groupRepository.AddMultiplyAsync(existingGroups);

        await this.dbContext.SaveChangesAsync();

        // Act
        List<Group> groups = await this.groupRepository.GetAllAsync();

        // Assert
        CollectionAssert.AreEquivalent(existingGroups, groups);
        Assert.AreEqual(10, groups.Count);
        Assert.IsTrue(existingGroups[3].Vehicles.Count > 0);
        Assert.AreEqual(vehicle, existingGroups[3].Vehicles[0]);
    }

    [TestMethod]
    public async Task Should_GetFiveAsync_ReturnGroupsWithVehicles_Successfuly()
    {
        // Arrange
        User tenant = Builder<User>.CreateNew()
            .With(t => t.FullName == "Tenant User")
            .With(t => t.UserName = $"tenantUser-{Guid.NewGuid()}")
            .With(t => t.Id == Guid.NewGuid()).Persist();
        tenant.AssociateTenant(tenant.Id);

        User userEmployee = Builder<User>.CreateNew()
            .With(uE => uE.FullName == "Employee User")
            .With(uE => uE.UserName = $"employeeUser-{Guid.NewGuid()}")
            .With(uE => uE.Id == Guid.NewGuid()).Persist();
        userEmployee.AssociateTenant(tenant.Id);

        List<Group> existingGroups = Builder<Group>.CreateListOfSize(10).Build().ToList();
        foreach (Group group in existingGroups)
        {
            group.AssociateTenant(tenant.Id);
            group.AssociateUser(userEmployee);
        }

        Vehicle vehicle = Builder<Vehicle>.CreateNew().Build();
        vehicle.AssociateTenant(tenant.Id);
        vehicle.AssociateUser(userEmployee);

        existingGroups[3].AddVehicle(vehicle);

        await this.groupRepository.AddMultiplyAsync(existingGroups);

        await this.dbContext.SaveChangesAsync();

        // Act
        List<Group> groups = await this.groupRepository.GetAllAsync(5);

        // Assert
        Assert.AreEqual(5, groups.Count);
        Assert.IsTrue(existingGroups[3].Vehicles.Count > 0);
        Assert.AreEqual(vehicle, existingGroups[3].Vehicles[0]);
    }

    [TestMethod]
    public async Task Should_GetGroupById_ReturnGroupWithVehicles_Successfuly()
    {
        // Arrange
        User tenant = Builder<User>.CreateNew()
            .With(t => t.FullName == "Tenant User")
            .With(t => t.UserName = $"tenantUser-{Guid.NewGuid()}")
            .With(t => t.Id == Guid.NewGuid()).Persist();
        tenant.AssociateTenant(tenant.Id);

        User userEmployee = Builder<User>.CreateNew()
            .With(uE => uE.FullName == "Employee User")
            .With(uE => uE.UserName = $"employeeUser-{Guid.NewGuid()}")
            .With(uE => uE.Id == Guid.NewGuid()).Persist();
        userEmployee.AssociateTenant(tenant.Id);

        Group group = Builder<Group>.CreateNew().Build();
        group.AssociateTenant(tenant.Id);
        group.AssociateUser(userEmployee);

        Vehicle vehicle = Builder<Vehicle>.CreateNew().Build();
        vehicle.AssociateTenant(tenant.Id);
        vehicle.AssociateUser(userEmployee);

        group.AddVehicle(vehicle);

        await this.groupRepository.AddAsync(group);
        await this.dbContext.SaveChangesAsync();
        // Act
        Group? selectedGroup = await this.groupRepository.GetByIdAsync(group.Id);

        // Assert
        Assert.IsNotNull(selectedGroup);
        Assert.AreEqual(group, selectedGroup);
        Assert.IsTrue(selectedGroup!.Vehicles.Count > 0);
        Assert.AreEqual(vehicle, selectedGroup.Vehicles[0]);
    }
}
