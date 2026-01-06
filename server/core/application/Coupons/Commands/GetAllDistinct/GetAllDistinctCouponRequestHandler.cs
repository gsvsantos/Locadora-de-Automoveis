using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAllAvailable;
using LocadoraDeAutomoveis.Domain.Coupons;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.GetAllDistinct;

public class GetAllDistinctCouponRequestHandler(
    IMapper mapper,
    IRepositoryCoupon repositoryCoupon,
    IRepositoryVehicle repositoryVehicle,
    IDistributedCache cache,
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

            string versionKey = "coupons:master-version";
            string? version = await cache.GetStringAsync(versionKey, cancellationToken);

            if (string.IsNullOrEmpty(version))
            {
                version = Guid.NewGuid().ToString();

                await cache.SetStringAsync(versionKey, version, cancellationToken);
            }

            string cacheKey = $"coupons:v={version}:t={tenantId}:distinct";

            string? jsonString = await cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                GetAllDistinctCouponResponse? cachedResult = JsonSerializer.Deserialize<GetAllDistinctCouponResponse>(jsonString);

                if (cachedResult is not null)
                {
                    return Result.Ok(cachedResult);
                }
            }

            List<Coupon> coupons = await repositoryCoupon.GetAllByTenantDistinctAsync(tenantId);

            GetAllDistinctCouponResponse response = mapper.Map<GetAllDistinctCouponResponse>(coupons);

            string jsonPayload = JsonSerializer.Serialize(response);

            DistributedCacheEntryOptions cacheOptions = new()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
            };

            await cache.SetStringAsync(cacheKey, jsonPayload, cacheOptions, cancellationToken);

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
