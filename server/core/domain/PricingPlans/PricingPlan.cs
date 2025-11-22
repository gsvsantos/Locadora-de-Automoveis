using LocadoraDeAutomoveis.Domain.Groups;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.PricingPlans;

public class PricingPlan : BaseEntity<PricingPlan>
{
    public decimal DailyRate { get; set; } = 0;
    public decimal PricePerKm { get; set; } = 0;
    public int AvailableKm { get; set; } = 0;
    public decimal DailyPrice { get; set; } = 0;
    public decimal PricePerKmExtrapolated { get; set; } = 0;
    public decimal FixedRate { get; set; } = 0;
    public Guid GroupId { get; set; } = Guid.Empty;
    public Group? Group { get; set; } = null;

    public PricingPlan() { }
    public PricingPlan(decimal dailyRate, decimal pricePerKm, int availableKm, decimal dailyPrice, decimal pricePerKmExtrapolated, decimal fixedRate)
    {
        this.DailyRate = dailyRate;
        this.PricePerKm = pricePerKm;
        this.AvailableKm = availableKm;
        this.DailyPrice = dailyPrice;
        this.PricePerKmExtrapolated = pricePerKmExtrapolated;
        this.FixedRate = fixedRate;
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
        this.DailyPrice = updatedEntity.DailyPrice;
        this.PricePerKm = updatedEntity.PricePerKm;
        this.AvailableKm = updatedEntity.AvailableKm;
        this.DailyRate = updatedEntity.DailyRate;
        this.PricePerKmExtrapolated = updatedEntity.PricePerKmExtrapolated;
        this.FixedRate = updatedEntity.FixedRate;
    }
}
