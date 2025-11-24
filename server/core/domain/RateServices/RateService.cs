using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.RateServices;

public class RateService : BaseEntity<RateService>
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; } = 0;
    public bool IsFixed { get; set; }

    public RateService() { }
    public RateService(string name, decimal price) : this()
    {
        this.Name = name;
        this.Price = price;
    }

    public void MarkAsFixed() => this.IsFixed = true;

    public void MarkAsDaily() => this.IsFixed = false;

    public override void Update(RateService updatedEntity)
    {
        this.Name = updatedEntity.Name;
        this.Price = updatedEntity.Price;
        this.IsFixed = updatedEntity.IsFixed;
    }
}
