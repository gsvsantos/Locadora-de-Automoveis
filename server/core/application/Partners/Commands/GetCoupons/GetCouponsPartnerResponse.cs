using LocadoraDeAutomoveis.Application.Partners.Commands.GetAll;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Partners.Commands.GetCoupons;

public record GetCouponsPartnerResponse(
    GetCouponsPartnerDto Partner
);

public record GetCouponsPartnerDto(
    Guid Id,
    string FullName,
    bool IsActive,
    ImmutableList<CouponDto> Coupons
) : PartnerDto(
    Id,
    FullName,
    IsActive
);

public record CouponDto(
    Guid Id,
    string Name,
    PartnerDto Partner,
    decimal DiscountValue,
    DateTimeOffset ExpirationDate,
    bool IsActive
);