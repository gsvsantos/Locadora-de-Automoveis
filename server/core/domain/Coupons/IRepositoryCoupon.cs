using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Coupons;

public interface IRepositoryCoupon : IRepository<Coupon>
{
    Task<bool> ExistsByPartnerId(Guid partnerId);

    Task<Coupon?> GetByNameAsync(string name);
}