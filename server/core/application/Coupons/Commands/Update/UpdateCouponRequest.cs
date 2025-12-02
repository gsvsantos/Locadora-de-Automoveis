using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.Update;

public record UpdateCouponRequestPartial(
    string Name,
    decimal DiscountValue,
    DateTimeOffset ExpirationDate,
    Guid PartnerId
);

public record UpdateCouponRequest(
    Guid Id,
    string Name,
    decimal DiscountValue,
    DateTimeOffset ExpirationDate,
    Guid PartnerId
) : IRequest<Result<UpdateCouponResponse>>;
