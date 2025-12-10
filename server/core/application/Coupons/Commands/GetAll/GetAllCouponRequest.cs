using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.GetAll;

public record GetAllCouponRequestPartial(
    int? Quantity,
    bool? IsActive
);

public record GetAllCouponRequest(
    int? Quantity,
    bool? IsActive
) : IRequest<Result<GetAllCouponResponse>>;
