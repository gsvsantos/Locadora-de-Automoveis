using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.Create;

public record CreateCouponRequest(
    string Name,
    decimal DiscountValue,
    DateTimeOffset ExpirationDate,
    Guid PartnerId
) : IRequest<Result<CreateCouponResponse>>;
