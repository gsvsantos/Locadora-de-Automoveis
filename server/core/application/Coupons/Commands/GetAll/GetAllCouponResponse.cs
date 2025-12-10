using LocadoraDeAutomoveis.Application.Partners.Commands.GetCoupons;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.GetAll;

public record GetAllCouponResponse(
    int Quantity,
    ImmutableList<CouponDto> Coupons
);
