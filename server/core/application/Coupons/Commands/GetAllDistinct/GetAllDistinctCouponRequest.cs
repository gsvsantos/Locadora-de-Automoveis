using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.GetAllDistinct;

public record GetAllDistinctCouponRequest(
    Guid VehicleId
) : IRequest<Result<GetAllDistinctCouponResponse>>;
