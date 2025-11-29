using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.PricingPlans;

public interface IRepositoryPricingPlan : IRepository<PricingPlan>
{
    Task<PricingPlan?> GetByGroupId(Guid id);
}
