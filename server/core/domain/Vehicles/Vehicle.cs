using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Vehicles;

public class Vehicle : BaseEntity<Vehicle>
{
    public string LicensePlate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public EFuelType FuelType { get; set; }
    public int FuelTankCapacity { get; set; } = 0;
    public decimal Kilometers { get; set; } = 0;
    public int Year { get; set; } = 0;
    public string? Image { get; set; }
    public Guid GroupId { get; set; } = Guid.Empty;
    public Group Group { get; set; } = null!;

    public Vehicle() { }
    public Vehicle(
        string licensePlate, string brand, string color, string model,
        int fuelTankCapacity, decimal kilometers, int year, string image) : this()
    {
        this.LicensePlate = licensePlate;
        this.Brand = brand;
        this.Color = color;
        this.Model = model;
        this.FuelTankCapacity = fuelTankCapacity;
        this.Kilometers = kilometers;
        this.Year = year;
        this.Image = image;
    }

    public void AssociateGroup(Group group)
    {
        if (this.Group is not null && this.Group.Id.Equals(group.Id))
        {
            return;
        }

        if (this.Group is not null)
        {
            DisassociateGroup();
        }

        group.AddVehicle(this);
        this.Group = group;
        this.GroupId = group.Id;
    }

    public void DisassociateGroup()
    {
        if (this.Group is null)
        {
            return;
        }

        this.Group.RemoveVehicle(this);
        this.Group = null!;
        this.GroupId = Guid.Empty;
    }

    public void SetFuelType(EFuelType fuelType)
    {
        this.FuelType = fuelType;
    }

    public void KilometersSum(decimal kilometersDriven)
    {
        this.Kilometers += kilometersDriven;
    }

    public override void Update(Vehicle updatedEntity)
    {
        this.LicensePlate = updatedEntity.LicensePlate;
        this.Brand = updatedEntity.Brand;
        this.Color = updatedEntity.Color;
        this.Model = updatedEntity.Model;
        this.FuelType = updatedEntity.FuelType;
        this.FuelTankCapacity = updatedEntity.FuelTankCapacity;
        this.Year = updatedEntity.Year;
        this.Image = updatedEntity.Image;

        if (updatedEntity.Group is not null && updatedEntity.Group.Id != this.Group.Id)
        {
            DisassociateGroup();
            AssociateGroup(updatedEntity.Group);
        }
    }
}

public enum EFuelType
{
    Gasoline,
    Gas,
    Diesel,
    Alcohol,
    Ethanol
}

public enum EVehicleStatus
{

}