using LocadoraDeAutomoveis.Domain.BillingPlans;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;

namespace LocadoraDeAutomoveis.Domain.Groups;

public class Group : BaseEntity<Group>
{
    public string Name { get; set; } = string.Empty;
    public List<Vehicle> Vehicles { get; set; } = [];
    public List<BillingPlan> BillingPlans { get; set; } = [];

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

    public void AddBillingPlan(BillingPlan BillingPlan)
    {
        this.BillingPlans.Add(BillingPlan);
    }

    public void RemoveBillingPlan(BillingPlan BillingPlan)
    {
        this.BillingPlans.Remove(BillingPlan);
    }

    public override void Update(Group updatedEntity)
    {
        this.Name = updatedEntity.Name;
    }
}
