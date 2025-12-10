using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.GetMostUsed;

public record GetMostUsedCouponRequest()
    : IRequest<Result<GetMostUsedCouponResponse>>;
