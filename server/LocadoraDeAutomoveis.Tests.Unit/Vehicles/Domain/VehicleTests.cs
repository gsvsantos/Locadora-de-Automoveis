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
        Assert.AreEqual(string.Empty, vehicle.FuelType);
        Assert.AreEqual(0, vehicle.CapacityInLiters);
        Assert.AreEqual(DateTimeOffset.MinValue, vehicle.Year);
        Assert.IsNull(vehicle.PhotoPath);
    }

    [TestMethod]
    public void VehicleConstructor_Parameterized_ShouldWorks()
    {
        // Arrange & Act
        DateTimeOffset date = new(1984, 1, 1, 0, 0, 0, TimeSpan.Zero);
        Vehicle vehicle = new(
            "ABC-1A84",
            "Chevrolet",
            "Preto",
            "Chevette",
            "Gasolina",
            45,
            date,
            string.Empty
        );

        // Assert
        Assert.AreNotEqual(Guid.Empty, vehicle.Id);
        Assert.IsTrue(vehicle.IsActive);
        Assert.AreEqual("ABC-1A84", vehicle.LicensePlate);
        Assert.AreEqual("Chevrolet", vehicle.Brand);
        Assert.AreEqual("Preto", vehicle.Color);
        Assert.AreEqual("Chevette", vehicle.Model);
        Assert.AreEqual("Gasolina", vehicle.FuelType);
        Assert.AreEqual(45, vehicle.CapacityInLiters);
        Assert.AreEqual(date, vehicle.Year);
        Assert.AreEqual(string.Empty, vehicle.PhotoPath);
    }

    [TestMethod]
    public void VehicleMethod_Update_ShouldWorks()
    {
        // Arrange
        DateTimeOffset date = new(1984, 1, day: 1, 0, 0, 0, TimeSpan.Zero);
        Vehicle vehicle = new(
            "ABC-1A84",
            "Chervrolette",
            "Preto",
            "Chevette",
            "Gasolina",
            45,
            new(2004, 1, 1, 0, 0, 0, TimeSpan.Zero),
            string.Empty
        );

        Vehicle updatedVehicle = new(
            "ABC-1A84",
            "Chevrolet",
            "Preto",
            "Chevette",
            "Gasolina",
            45,
            date,
            "https://aquelesite.net/ze"
        );

        // Act
        vehicle.Update(updatedVehicle);

        // Assert
        Assert.AreNotEqual(Guid.Empty, vehicle.Id);
        Assert.IsTrue(vehicle.IsActive);
        Assert.AreEqual("ABC-1A84", vehicle.LicensePlate);
        Assert.AreEqual("Chevrolet", vehicle.Brand);
        Assert.AreEqual("Preto", vehicle.Color);
        Assert.AreEqual("Chevette", vehicle.Model);
        Assert.AreEqual("Gasolina", vehicle.FuelType);
        Assert.AreEqual(45, vehicle.CapacityInLiters);
        Assert.AreEqual(date, vehicle.Year);
        Assert.AreEqual("https://aquelesite.net/ze", vehicle.PhotoPath);
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
