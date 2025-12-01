using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Partners;

public class Partner : BaseEntity<Partner>
{
    public string FullName { get; set; } = string.Empty;
    public List<Coupon> Coupons { get; set; } = [];

    public Partner() { }
    public Partner(string fullName) : this() => this.FullName = fullName;

    public override void Update(Partner updatedEntity) => this.FullName = updatedEntity.FullName;
}
