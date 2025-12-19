using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.GetAllAvailable;

public record GetAllAvailableCouponRequest(
) : IRequest<Result<GetAllAvailableCouponResponse>>;
