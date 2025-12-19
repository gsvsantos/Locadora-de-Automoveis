using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAllAvailable;
using LocadoraDeAutomoveis.Domain.Coupons;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.GetAllAvailable;

public class GetAllAvailableCouponRequestHandler(
    IRepositoryCoupon repositoryCoupon,
    IMapper mapper,
    ILogger<GetAllAvailableVehiclesRequestHandler> logger
) : IRequestHandler<GetAllAvailableCouponRequest, Result<GetAllAvailableCouponResponse>>
{
    public async Task<Result<GetAllAvailableCouponResponse>> Handle(
        GetAllAvailableCouponRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Coupon> coupons = await repositoryCoupon.GetAllAvailableAsync();

            GetAllAvailableCouponResponse response = mapper.Map<GetAllAvailableCouponResponse>(coupons);

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
