using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Coupons;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.GetById;

public class GetByIdCouponRequestHandler(
    IMapper mapper,
    IRepositoryCoupon repositoryCoupon,
    ILogger<GetByIdCouponRequestHandler> logger
) : IRequestHandler<GetByIdCouponRequest, Result<GetByIdCouponResponse>>
{
    public async Task<Result<GetByIdCouponResponse>> Handle(
        GetByIdCouponRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Coupon? selectedCoupon = await repositoryCoupon.GetByIdAsync(request.Id);

            if (selectedCoupon == null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.Id));
            }

            GetByIdCouponResponse response = mapper.Map<GetByIdCouponResponse>(selectedCoupon);

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error retrieving coupon by ID {CouponId}", request.Id);

            return Result.Fail(new Error("An error occurred while retrieving the coupon."));
        }
    }
}