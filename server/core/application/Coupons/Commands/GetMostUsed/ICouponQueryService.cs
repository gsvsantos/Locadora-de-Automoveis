namespace LocadoraDeAutomoveis.Application.Coupons.Commands.GetMostUsed;

public interface ICouponQueryService
{
    Task<List<CouponUsageDto>> GetMostUsedCouponsAsync();
}
