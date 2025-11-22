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
    public string PhotoPath { get; set; }
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;

    public Vehicle(
        string licensePlate, string brand, string color, string model,
        string fuelType, int capacityInLiters, DateTimeOffset year, string photoPath
    )
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

        group.Vehicles.Add(this);
        this.Group = group;
        this.GroupId = group.Id;
    }

    public override void Update(Vehicle updatedEntity) => throw new NotImplementedException();
}
