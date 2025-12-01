using LocadoraDeAutomoveis.Domain.Partners;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Coupons;

public class Coupon : BaseEntity<Coupon>
{

    public string Name { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public DateTimeOffset ExpirationDate { get; set; }
    public Guid PartnerId { get; set; }
    public Partner Partner { get; set; } = null!;
    public bool IsManuallyDisabled { get; set; }

    public Coupon() { }
    public Coupon(string name, decimal discountValue, DateTimeOffset expirationDate) : this()
    {
        this.Name = name;
        this.DiscountValue = discountValue;
        this.ExpirationDate = expirationDate;
    }

    public void SetManuallyDisabledTrue() => this.IsManuallyDisabled = true;

    public bool IsExpired() => DateTimeOffset.UtcNow > this.ExpirationDate;

    public void AssociatePartner(Partner partner)
    {
        this.Partner = partner;
        this.PartnerId = partner.Id;
    }

    public void DisassociatePartner()
    {
        if (this.Partner is null)
        {
            return;
        }

        this.Partner = null!;
        this.PartnerId = Guid.Empty;
    }

    public override void Update(Coupon updatedEntity)
    {
        this.Name = updatedEntity.Name;
        this.DiscountValue = updatedEntity.DiscountValue;
        this.ExpirationDate = updatedEntity.ExpirationDate;
        this.PartnerId = updatedEntity.PartnerId;
    }
}
