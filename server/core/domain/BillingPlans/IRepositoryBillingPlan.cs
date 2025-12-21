using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.BillingPlans;

public interface IRepositoryBillingPlan : IRepository<BillingPlan>
{
    Task<bool> ExistsByGroupId(Guid groupId);

    Task<BillingPlan?> GetByGroupId(Guid id);

    Task<BillingPlan?> GetByTenantAndGroupAsync(Guid tenantId, Guid groupId);
}
