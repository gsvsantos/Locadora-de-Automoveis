using FluentResults;
using MediatR;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.GetMostUsed;

public class GetMostUsedCouponsRequestHandler(
    ICouponQueryService couponQueryService
) : IRequestHandler<GetMostUsedCouponsRequest, Result<GetMostUsedCouponsResponse>>
{
    public async Task<Result<GetMostUsedCouponsResponse>> Handle(
        GetMostUsedCouponsRequest request, CancellationToken cancellationToken)
    {
        List<CouponUsageDto> stats = await couponQueryService.GetMostUsedCouponsAsync();

        return Result.Ok(new GetMostUsedCouponsResponse(stats.ToImmutableList()));
    }
}
