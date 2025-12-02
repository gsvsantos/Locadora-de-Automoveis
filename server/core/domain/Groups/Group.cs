using LocadoraDeAutomoveis.Domain.PricingPlans;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;

namespace LocadoraDeAutomoveis.Domain.Groups;

public class Group : BaseEntity<Group>
{
    public string Name { get; set; } = string.Empty;
    public List<Vehicle> Vehicles { get; set; } = [];
    public List<PricingPlan> PricingPlans { get; set; } = [];

    public Group() { }
    public Group(string name) : this()
    {
        this.Name = name;
    }

    public void AddVehicle(Vehicle vehicle)
    {
        this.Vehicles.Add(vehicle);
    }

    public void RemoveVehicle(Vehicle vehicle)
    {
        this.Vehicles.Remove(vehicle);
    }

    public void AddPricingPlan(PricingPlan pricingPlan)
    {
        this.PricingPlans.Add(pricingPlan);
    }

    public void RemovePricingPlan(PricingPlan pricingPlan)
    {
        this.PricingPlans.Remove(pricingPlan);
    }

    public override void Update(Group updatedEntity)
    {
        this.Name = updatedEntity.Name;
    }
}
