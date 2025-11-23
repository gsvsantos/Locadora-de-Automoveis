using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.PricingPlans;

public class PricingPlan : BaseEntity<PricingPlan>
{
    public DailyPlanProps DailyPlan { get; set; }
    public ControlledPlanProps ControlledPlan { get; set; }
    public FreePlanProps FreePlan { get; set; }
    public Guid GroupId { get; set; } = Guid.Empty;
    public Group Group { get; set; } = null!;

    public PricingPlan() { }
    public PricingPlan(DailyPlanProps dailyPlanProps, ControlledPlanProps controlledPlanProps, FreePlanProps freePlanProps) : this()
    {
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
        this.DailyPlan = updatedEntity.DailyPlan;
        this.ControlledPlan = updatedEntity.ControlledPlan;
        this.FreePlan = updatedEntity.FreePlan;
    }
}

public record DailyPlanProps(decimal DailyRate, decimal PricePerKm);
public record ControlledPlanProps(decimal DailyRate, decimal PricePerKmExtrapolated, int AvailableKm);
public record FreePlanProps(decimal FixedRate);
