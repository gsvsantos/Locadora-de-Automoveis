using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Coupons;

public interface IRepositoryCoupon : IRepository<Coupon>
{
    Task<Coupon?> GetByNameAsync(string name);
}