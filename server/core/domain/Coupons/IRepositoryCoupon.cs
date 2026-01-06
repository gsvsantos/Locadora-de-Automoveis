using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Coupons;

public interface IRepositoryCoupon : IRepository<Coupon>
{
    Task<bool> ExistsByPartnerId(Guid partnerId);

    Task<Coupon?> GetByNameAsync(string name);

    Task<List<Coupon>> SearchAsync(string term, CancellationToken ct = default);

    Task<List<Coupon>> GetAllByTenantDistinctAsync(Guid tenantId);

    Task<Coupon?> GetByTenantAndIdAsync(Guid tenantId, Guid entityId);

}