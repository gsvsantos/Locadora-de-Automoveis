using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Rentals;

public class RentalReturn : BaseEntity<RentalReturn>
{
    public DateTimeOffset ReturnDate { get; set; }
    public decimal EndKm { get; set; }
    public decimal TotalMileage { get; set; }
    public decimal ExtrasTotalCost { get; set; }
    public decimal FuelPenalty { get; set; }
    public decimal PenaltyTotalCost { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal FinalPrice { get; set; }
    public EFuelLevel FuelLevelAtReturn { get; set; }
    public Guid RentalId { get; set; }
    public Rental Rental { get; set; } = null!;

    public RentalReturn() { }
    public RentalReturn(DateTimeOffset returnDate, decimal endKm, decimal totalMileage
    ) : this()
    {
        this.ReturnDate = returnDate;
        this.EndKm = endKm;
        this.TotalMileage = totalMileage;
    }

    public void SetFuelLevel(EFuelLevel fuelLevel)
    {
        this.FuelLevelAtReturn = fuelLevel;
    }

    public void SetExtrasTotalCost(decimal extrasTotal)
    {
        this.ExtrasTotalCost = extrasTotal;
    }

    public void SetFuelPenalty(decimal fuelPenality)
    {
        this.FuelPenalty = fuelPenality;
    }

    public void SetPenaltyTotal(decimal penaltyTotal)
    {
        this.PenaltyTotalCost = penaltyTotal;
    }

    public void SetDiscountTotal(decimal discount)
    {
        this.DiscountTotal = discount;
    }

    public void SetFinalPrice(decimal finalPrice)
    {
        this.FinalPrice = finalPrice;
    }

    public void AssociateRental(Rental rental)
    {
        this.Rental = rental;
        this.RentalId = rental.Id;
    }

    public override void Update(RentalReturn updatedEntity)
    {
        // update isnt necessary =)
        // override only for the others methods and props from baseentity =]
    }
}

public enum EFuelLevel
{
    Empty = 0,
    Quarter = 25,
    Half = 50,
    ThreeQuarters = 75,
    Full = 100
}
