using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Partners;

public class Partner : BaseEntity<Partner>
{
    public string FullName { get; set; } = string.Empty;
    public List<Coupon> Coupons { get; set; } = [];

    public Partner() { }
    public Partner(string fullName) : this()
    {
        this.FullName = fullName;
    }

    public void AddRangeCoupons(List<Coupon> coupons)
    {
        List<Coupon> newCoupons = coupons
        .Where(newC => !this.Coupons.Any(existing =>
            existing.Id.Equals(newC.Id))
        )
        .ToList();

        if (newCoupons.Count > 0)
        {
            this.Coupons.AddRange(newCoupons);
        }
    }

    public void AddCoupon(Coupon coupon)
    {
        if (this.Coupons.Any(c => c.Equals(coupon)))
        {
            return;
        }

        this.Coupons.Add(coupon);
    }

    public void RemoveCoupon(Coupon coupon)
    {
        this.Coupons.Remove(coupon);
    }

    public override void Update(Partner updatedEntity)
    {
        this.FullName = updatedEntity.FullName;
    }
}
