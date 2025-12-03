using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.GetMostUsed;

public record GetMostUsedCouponsResponse(ImmutableList<CouponUsageDto> Data);

public record CouponUsageDto(
    string Name,
    string PartnerName,
    int UsageCount,
    decimal TotalDiscountGiven
);