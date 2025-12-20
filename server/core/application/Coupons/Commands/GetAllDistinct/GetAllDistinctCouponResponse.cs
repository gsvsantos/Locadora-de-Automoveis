using LocadoraDeAutomoveis.Application.Partners.Commands.GetCoupons;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.GetAllAvailable;

public record GetAllDistinctCouponResponse(
    int Quantity,
    ImmutableList<CouponDto> Coupons
);
