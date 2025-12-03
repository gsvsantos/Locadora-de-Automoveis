using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.GetMostUsed;

public record GetMostUsedCouponsRequest()
    : IRequest<Result<GetMostUsedCouponsResponse>>;
