using LocadoraDeAutomoveis.Application.Coupons.Commands.GetMostUsed;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Coupons;

public class CouponQueryService(AppDbContext context) : ICouponQueryService
{
    public async Task<List<CouponUsageDto>> GetMostUsedCouponsAsync()
    {
        List<Rental> rentalsWithCoupons = await context.Rentals
        .Include(r => r.Coupon)
            .ThenInclude(c => c!.Partner)
        .Where(r => r.CouponId != null)
        .ToListAsync();

        List<CouponUsageDto> report = rentalsWithCoupons
            .GroupBy(r => r.CouponId)
            .Select(group =>
            {
                Rental rental = group.First();

                return new CouponUsageDto(
                    rental.Coupon!.Name,
                    rental.Coupon.Partner.FullName,
                    group.Count(),
                    group.Sum(r => r.Coupon!.DiscountValue)
                );
            })
            .OrderByDescending(dto => dto.UsageCount)
            .ToList();

        return report;
    }
}