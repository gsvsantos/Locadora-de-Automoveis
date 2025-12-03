using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.PricingPlans;

public interface IRepositoryPricingPlan : IRepository<PricingPlan>
{
    Task<bool> ExistsByGroupId(Guid groupId);

    Task<PricingPlan?> GetByGroupId(Guid id);
}
