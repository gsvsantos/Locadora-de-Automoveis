using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.RateServices;

public class RateService : BaseEntity<RateService>
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; } = 0;
    public bool IsChargedPerDay { get; set; }
    public ERateType ERateType { get; set; }

    public RateService() { }
    public RateService(string name, decimal price) : this()
    {
        this.Name = name;
        this.Price = price;
    }

    public void MarkAsFixed() => this.IsChargedPerDay = true;

    public void MarkAsDaily() => this.IsChargedPerDay = false;

    public void MarkAsGenericType() => this.ERateType = ERateType.Generic;

    public void MarkAsInsuranceClientType() => this.ERateType = ERateType.InsuranceClient;

    public void MarkAsInsuranceThirdPartyType() => this.ERateType = ERateType.InsuranceThirdParty;

    public override void Update(RateService updatedEntity)
    {
        this.Name = updatedEntity.Name;
        this.Price = updatedEntity.Price;
        this.IsChargedPerDay = updatedEntity.IsChargedPerDay;
    }
}

public enum ERateType
{
    Generic,
    InsuranceClient,
    InsuranceThirdParty
}