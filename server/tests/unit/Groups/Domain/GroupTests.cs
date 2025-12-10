using FizzWare.NBuilder;
using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Vehicles;

namespace LocadoraDeAutomoveis.Tests.Unit.Groups.Domain;

[TestClass]
[TestCategory("Group Domain - Unit Tests")]
public sealed class GroupTests
{
    [TestMethod]
    public void GroupConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        Group group = new();

        // Assert
        Assert.AreNotEqual(Guid.Empty, group.Id);
        Assert.IsTrue(group.IsActive);
        Assert.AreEqual(string.Empty, group.Name);
        Assert.AreEqual(0, group.Vehicles.Count);
    }

    [TestMethod]
    public void GroupConstructor_Parameterized_ShouldWorks()
    {
        // Arrange & Act
        Group group = new("Grupo");

        // Assert
        Assert.AreNotEqual(Guid.Empty, group.Id);
        Assert.IsTrue(group.IsActive);
        Assert.AreEqual("Grupo", group.Name);
        Assert.AreEqual(0, group.Vehicles.Count);
    }

    [TestMethod]
    public void GroupMethod_Update_ShouldWorks()
    {
        // Arrange
        Group group = new("Grupo");
        Group updatedGroup = new("Grupo Atualizado");

        // Act
        group.Update(updatedGroup);

        // Assert
        Assert.AreEqual("Grupo Atualizado", group.Name);
    }

    [TestMethod]
    public void GroupMethod_AddVehicle_ShouldWorks()
    {
        // Arrange
        Group group = new("Grupo");
        Vehicle vehicle = Builder<Vehicle>.CreateNew().Build();

        // Act
        group.AddVehicle(vehicle);

        // Assert
        Assert.AreEqual(1, group.Vehicles.Count);
        Assert.AreEqual(vehicle, group.Vehicles[0]);
    }

    [TestMethod]
    public void GroupMethod_RemoveVehicle_ShouldWorks()
    {
        // Arrange
        Group group = new("Grupo");
        Vehicle vehicle = Builder<Vehicle>.CreateNew().Build();

        group.AddVehicle(vehicle);

        // Act
        group.RemoveVehicle(vehicle);

        // Assert
        Assert.AreEqual(0, group.Vehicles.Count);
    }
}
