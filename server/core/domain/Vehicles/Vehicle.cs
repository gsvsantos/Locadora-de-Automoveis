using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Vehicles;

public class Vehicle : BaseEntity<Vehicle>
{
    public string LicensePlate { get; set; }
    public string Brand { get; set; }
    public string Color { get; set; }
    public string Model { get; set; }
    public string FuelType { get; set; }
    public int CapacityInLiters { get; set; }
    public DateTimeOffset Year { get; set; }
    public string PhotoPath { get; set; } // não é obrigatóio, pode ser nulo ou vazio
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;

    public Vehicle() { }
    public Vehicle(
        string licensePlate, string brand, string color, string model,
        string fuelType, int capacityInLiters, DateTimeOffset year, string photoPath
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
        if (this.Group is not null)
        {
            return;
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
