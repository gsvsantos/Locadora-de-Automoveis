using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Vehicles;

public class Vehicle : BaseEntity<Vehicle>
{
    public string LicensePlate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public int CapacityInLiters { get; set; } = 0;
    public int Year { get; set; } = 0;
    public string? PhotoPath { get; set; }
    public Guid GroupId { get; set; } = Guid.Empty;
    public Group Group { get; set; } = null!;

    public Vehicle() { }
    public Vehicle(
        string licensePlate, string brand, string color, string model,
        string fuelType, int capacityInLiters, int year, string photoPath
    ) : this()
    {
        this.LicensePlate = licensePlate;
        this.Brand = brand;
        this.Color = color;
        this.Model = model;
        this.FuelType = fuelType;
        this.CapacityInLiters = capacityInLiters;
        this.Year = year;
        this.PhotoPath = photoPath;
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

    public override void Update(Vehicle updatedEntity)
    {
        this.LicensePlate = updatedEntity.LicensePlate;
        this.Brand = updatedEntity.Brand;
        this.Color = updatedEntity.Color;
        this.Model = updatedEntity.Model;
        this.FuelType = updatedEntity.FuelType;
        this.CapacityInLiters = updatedEntity.CapacityInLiters;
        this.Year = updatedEntity.Year;
        this.PhotoPath = updatedEntity.PhotoPath;

        if (updatedEntity.Group is not null && updatedEntity.Group.Id != this.Group.Id)
        {
            DisassociateGroup();
            AssociateGroup(updatedEntity.Group);
        }
    }
}
