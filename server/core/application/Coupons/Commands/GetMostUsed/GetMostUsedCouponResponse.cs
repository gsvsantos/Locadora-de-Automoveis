using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.GetMostUsed;

public record GetMostUsedCouponResponse(
    int Quantity,
    ImmutableList<CouponUsageDto> Coupons
);

public record CouponUsageDto(
    Guid Id,
    string Name,
    string PartnerName,
    int UsageCount,
    decimal TotalDiscountGiven
);