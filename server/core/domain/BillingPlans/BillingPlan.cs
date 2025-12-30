using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.BillingPlans;

public class BillingPlan : BaseEntity<BillingPlan>
{
    public string Name { get; set; } = string.Empty;
    public DailyBilling Daily { get; set; } = null!;
    public ControlledBilling Controlled { get; set; } = null!;
    public FreeBilling Free { get; set; } = null!;
    public Guid GroupId { get; set; } = Guid.Empty;
    public Group Group { get; set; } = null!;

    public BillingPlan() { }
    public BillingPlan(string name, DailyBilling daily, ControlledBilling controlled, FreeBilling free) : this()
    {
        this.Name = name;
        this.Daily = daily;
        this.Controlled = controlled;
        this.Free = free;
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

        group.AddBillingPlan(this);
        this.Group = group;
        this.GroupId = group.Id;
    }

    public void DisassociateGroup()
    {
        if (this.Group is null)
        {
            return;
        }

        this.Group.RemoveBillingPlan(this);
        this.Group = null!;
        this.GroupId = Guid.Empty;
    }

    public override void Update(BillingPlan updatedEntity)
    {
        this.Name = updatedEntity.Name;
        this.Daily = updatedEntity.Daily;
        this.Controlled = updatedEntity.Controlled;
        this.Free = updatedEntity.Free;
    }
}

public record DailyBilling(decimal DailyRate, decimal PricePerKm);
public record ControlledBilling(decimal DailyRate, decimal PricePerKmExtrapolated);
public record FreeBilling(decimal FixedRate);
