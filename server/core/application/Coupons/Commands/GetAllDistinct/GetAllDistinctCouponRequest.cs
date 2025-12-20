using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.GetAllAvailable;

public record GetAllDistinctCouponRequest(
    Guid VehicleId
) : IRequest<Result<GetAllDistinctCouponResponse>>;
