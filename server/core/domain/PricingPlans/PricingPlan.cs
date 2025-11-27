using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.PricingPlans;

public class PricingPlan : BaseEntity<PricingPlan>
{
    public string Name { get; set; } = string.Empty;
    public DailyPlanProps DailyPlan { get; set; } = null!;
    public ControlledPlanProps ControlledPlan { get; set; } = null!;
    public FreePlanProps FreePlan { get; set; } = null!;
    public Guid GroupId { get; set; } = Guid.Empty;
    public Group Group { get; set; } = null!;

    public PricingPlan() { }
    public PricingPlan(string name, DailyPlanProps dailyPlanProps, ControlledPlanProps controlledPlanProps, FreePlanProps freePlanProps) : this()
    {
        this.Name = name;
        this.DailyPlan = dailyPlanProps;
        this.ControlledPlan = controlledPlanProps;
        this.FreePlan = freePlanProps;
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

        group.AddPricingPlan(this);
        this.Group = group;
        this.GroupId = group.Id;
    }

    public void DisassociateGroup()
    {
        if (this.Group is null)
        {
            return;
        }

        this.Group.RemovePricingPlan(this);
        this.Group = null!;
        this.GroupId = Guid.Empty;
    }

    public override void Update(PricingPlan updatedEntity)
    {
        this.Name = updatedEntity.Name;
        this.DailyPlan = updatedEntity.DailyPlan;
        this.ControlledPlan = updatedEntity.ControlledPlan;
        this.FreePlan = updatedEntity.FreePlan;
    }
}

public abstract record AvailablePlan;
public record DailyPlanProps(decimal DailyRate, decimal PricePerKm) : AvailablePlan;
public record ControlledPlanProps(decimal DailyRate, decimal PricePerKmExtrapolated) : AvailablePlan;
public record FreePlanProps(decimal FixedRate) : AvailablePlan;
