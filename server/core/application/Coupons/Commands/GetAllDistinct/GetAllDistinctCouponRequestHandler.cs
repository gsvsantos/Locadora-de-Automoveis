using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAllAvailable;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.GetAllDistinct;

public class GetAllDistinctCouponRequestHandler(
    IMapper mapper,
    IRepositoryCoupon repositoryCoupon,
    IRepositoryVehicle repositoryVehicle,
    ILogger<GetAllAvailableVehiclesRequestHandler> logger
) : IRequestHandler<GetAllDistinctCouponRequest, Result<GetAllDistinctCouponResponse>>
{
    public async Task<Result<GetAllDistinctCouponResponse>> Handle(
        GetAllDistinctCouponRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Vehicle? vehicle = await repositoryVehicle.GetByIdDistinctAsync(request.VehicleId);

            if (vehicle is null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.VehicleId));
            }

            Guid tenantId = vehicle.GetTenantId();

            List<Coupon> coupons = await repositoryCoupon.GetAllByTenantDistinctAsync(tenantId);

            GetAllDistinctCouponResponse response = mapper.Map<GetAllDistinctCouponResponse>(coupons);

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
