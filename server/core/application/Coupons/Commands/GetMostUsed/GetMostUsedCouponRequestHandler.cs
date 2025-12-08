using FluentResults;
using MediatR;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.GetMostUsed;

public class GetMostUsedCouponRequestHandler(
    ICouponQueryService couponQueryService
) : IRequestHandler<GetMostUsedCouponRequest, Result<GetMostUsedCouponResponse>>
{
    public async Task<Result<GetMostUsedCouponResponse>> Handle(
        GetMostUsedCouponRequest request, CancellationToken cancellationToken)
    {
        List<CouponUsageDto> coupons = await couponQueryService.GetMostUsedCouponsAsync();
        // todo: melhorar sistema dessa query, implementar mapper
        return Result.Ok(new GetMostUsedCouponResponse(coupons.Count, coupons.ToImmutableList()));
    }
}
