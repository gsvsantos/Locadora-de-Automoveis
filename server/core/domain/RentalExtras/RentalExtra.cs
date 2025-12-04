using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.RentalExtras;

public class RentalExtra : BaseEntity<RentalExtra>
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; } = 0;
    public bool IsDaily { get; set; }
    public EExtraType Type { get; set; }

    public RentalExtra() { }
    public RentalExtra(string name, decimal price) : this()
    {
        this.Name = name;
        this.Price = price;
    }

    public void MarkAsFixed()
    {
        this.IsDaily = true;
    }

    public void MarkAsDaily()
    {
        this.IsDaily = false;
    }

    public void DefineType(EExtraType rateType)
    {
        this.Type = rateType;
    }

    public override void Update(RentalExtra updatedEntity)
    {
        this.Name = updatedEntity.Name;
        this.Price = updatedEntity.Price;
        this.IsDaily = updatedEntity.IsDaily;
        DefineType(updatedEntity.Type);
    }
}

public enum EExtraType
{
    Equipment,
    Service,
    Fee
}