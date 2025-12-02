using LocadoraDeAutomoveis.Application.Partners.Commands.GetAll;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Partners.Commands.GetCoupons;

public record GetCouponsPartnerResponse(
    GetCouponsPartnerDto Partner
);

public record GetCouponsPartnerDto(
    Guid Id,
    string FullName,
    ImmutableList<CouponDto> Coupons
) : PartnerDto(
    Id,
    FullName
);

public record CouponDto(
    Guid Id,
    string Name,
    decimal DiscountValue,
    DateTimeOffset ExpirationDate
);