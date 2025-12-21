using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Vehicles;

namespace LocadoraDeAutomoveis.Tests.Unit.Vehicles.Domain;

[TestClass]
[TestCategory("Vehicle Domain - Unit Tests")]
public sealed class VehicleTests
{
    [TestMethod]
    public void VehicleConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        Vehicle vehicle = new();

        // Assert
        Assert.AreNotEqual(Guid.Empty, vehicle.Id);
        Assert.IsTrue(vehicle.IsActive);
        Assert.AreEqual(string.Empty, vehicle.LicensePlate);
        Assert.AreEqual(string.Empty, vehicle.Brand);
        Assert.AreEqual(string.Empty, vehicle.Color);
        Assert.AreEqual(string.Empty, vehicle.Model);
        Assert.AreEqual(EFuelType.Gasoline, vehicle.FuelType);
        Assert.AreEqual(0, vehicle.FuelTankCapacity);
        Assert.AreEqual(0, vehicle.Year);
        Assert.IsNull(vehicle.Image);
    }

    [TestMethod]
    public void VehicleConstructor_Parameterized_ShouldWorks()
    {
        // Arrange & Act
        Vehicle vehicle = new(
            "ABC-1A84",
            "Chevrolet",
            "Preto",
            "Chevette",
            45,
            1000,
            1984,
            string.Empty
        );
        vehicle.SetFuelType(EFuelType.Gasoline);

        // Assert
        Assert.AreNotEqual(Guid.Empty, vehicle.Id);
        Assert.IsTrue(vehicle.IsActive);
        Assert.AreEqual("ABC-1A84", vehicle.LicensePlate);
        Assert.AreEqual("Chevrolet", vehicle.Brand);
        Assert.AreEqual("Preto", vehicle.Color);
        Assert.AreEqual("Chevette", vehicle.Model);
        Assert.AreEqual(EFuelType.Gasoline, vehicle.FuelType);
        Assert.AreEqual(45, vehicle.FuelTankCapacity);
        Assert.AreEqual(1984, vehicle.Year);
        Assert.AreEqual(string.Empty, vehicle.Image);
    }

    [TestMethod]
    public void VehicleMethod_Update_ShouldWorks()
    {
        // Arrange
        Vehicle vehicle = new(
            "ABC-1A84",
            "Chervrolette",
            "Preto",
            "Chevette",
            45,
            500,
            2004,
            string.Empty
        );
        vehicle.SetFuelType(EFuelType.Gas);

        Vehicle updatedVehicle = new(
            "ABC-1A84",
            "Chevrolet",
            "Preto",
            "Chevette",
            45,
            1000,
            1984,
            "https://aquelesite.net/ze"
        );
        vehicle.SetFuelType(EFuelType.Gasoline);

        // Act
        vehicle.Update(updatedVehicle);

        // Assert
        Assert.AreNotEqual(Guid.Empty, vehicle.Id);
        Assert.IsTrue(vehicle.IsActive);
        Assert.AreEqual("ABC-1A84", vehicle.LicensePlate);
        Assert.AreEqual("Chevrolet", vehicle.Brand);
        Assert.AreEqual("Preto", vehicle.Color);
        Assert.AreEqual("Chevette", vehicle.Model);
        Assert.AreEqual(EFuelType.Gasoline, vehicle.FuelType);
        Assert.AreEqual(45, vehicle.FuelTankCapacity);
        Assert.AreEqual(1000, vehicle.Kilometers);
        Assert.AreEqual(1984, vehicle.Year);
        Assert.AreEqual("https://aquelesite.net/ze", vehicle.Image);
    }

    [TestMethod]
    public void VehicleMethod_Update_ShouldIgnoreSameGroup()
    {
        // Arrange
        Vehicle vehicle = new();

        Group group = new("Grupo");

        vehicle.AssociateGroup(group);

        Vehicle updatedVehicle = new();
        updatedVehicle.AssociateGroup(group);

        // Act
        vehicle.Update(updatedVehicle);

        // Assert
        Assert.AreEqual(group.Id, vehicle.GroupId);
        Assert.AreEqual(group, vehicle.Group);
        Assert.IsTrue(group.Vehicles.Contains(vehicle));
    }

    [TestMethod]
    public void VehicleMethod_AssociateGroup_ShouldWorks()
    {
        // Arrange
        Vehicle vehicle = new();

        Group group = new("Grupo");

        // Act
        vehicle.AssociateGroup(group);

        // Assert
        Assert.AreEqual(group.Id, vehicle.GroupId);
        Assert.AreEqual(group, vehicle.Group);
        Assert.IsTrue(group.Vehicles.Contains(vehicle));
    }

    [TestMethod]
    public void VehicleMethod_DisassociateGroup_ShouldWorks()
    {
        // Arrange
        Vehicle vehicle = new();

        Group group = new("Grupo");

        vehicle.AssociateGroup(group);

        // Act
        vehicle.DisassociateGroup();

        // Assert
        Assert.AreEqual(Guid.Empty, vehicle.GroupId);
        Assert.IsNull(vehicle.Group);
        Assert.IsFalse(group.Vehicles.Contains(vehicle));
    }
}
