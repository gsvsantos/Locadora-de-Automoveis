using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Coupons;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.GetAll;

public class GetAllCouponRequestHandler(
    IMapper mapper,
    IRepositoryCoupon repositoryCoupon,
    ILogger<GetAllCouponRequestHandler> logger
) : IRequestHandler<GetAllCouponRequest, Result<GetAllCouponResponse>>
{
    public async Task<Result<GetAllCouponResponse>> Handle(
        GetAllCouponRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Coupon> coupons = [];
            bool quantityProvided = request.Quantity.HasValue && request.Quantity.Value > 0;
            bool inactiveProvided = request.IsActive.HasValue;

            if (quantityProvided && inactiveProvided)
            {
                int quantity = request.Quantity!.Value;
                bool isActive = request.IsActive!.Value;

                coupons = await repositoryCoupon.GetAllAsync(quantity, isActive);
            }
            else if (inactiveProvided)
            {
                bool isActive = request.IsActive!.Value;

                coupons = await repositoryCoupon.GetAllAsync(isActive);
            }
            else if (quantityProvided)
            {
                int quantity = request.Quantity!.Value;

                coupons = await repositoryCoupon.GetAllAsync(quantity);
            }
            else
            {
                coupons = await repositoryCoupon.GetAllAsync(true);
            }

            GetAllCouponResponse response = mapper.Map<GetAllCouponResponse>(coupons);

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred during the request. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}