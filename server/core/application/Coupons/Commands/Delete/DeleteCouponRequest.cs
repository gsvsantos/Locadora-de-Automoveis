using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.Delete;

public record DeleteCouponRequest(
    Guid Id
) : IRequest<Result<DeleteCouponResponse>>;
