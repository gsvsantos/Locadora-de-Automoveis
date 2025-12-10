using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Configurations;

public class Configuration : BaseEntity<Configuration>
{
    public decimal GasolinePrice { get; set; } = 0;
    public decimal GasPrice { get; set; } = 0;
    public decimal DieselPrice { get; set; } = 0;
    public decimal AlcoholPrice { get; set; } = 0;

    public Configuration() { }
    public Configuration(decimal gasolinePrice, decimal gasPrice,
        decimal dieselPrice, decimal alcoholPrice
    ) : this()
    {
        this.GasolinePrice = gasolinePrice;
        this.GasPrice = gasPrice;
        this.DieselPrice = dieselPrice;
        this.AlcoholPrice = alcoholPrice;
    }

    public override void Update(Configuration updatedEntity)
    {
        this.GasolinePrice = updatedEntity.GasolinePrice;
        this.GasPrice = updatedEntity.GasPrice;
        this.DieselPrice = updatedEntity.DieselPrice;
        this.AlcoholPrice = updatedEntity.AlcoholPrice;
    }
}
