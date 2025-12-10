using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.GetById;

public record GetByIdCouponRequest(
    Guid Id
) : IRequest<Result<GetByIdCouponResponse>>;
