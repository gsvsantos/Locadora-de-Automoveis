using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LocadoraDeAutomoveis.Application.Drivers.Commands.GetAllDistinct;

public class GetAllDistinctDriverRequestHandler(
    IMapper mapper,
    IRepositoryDriver repositoryDriver,
    IRepositoryVehicle repositoryVehicle,
    IDistributedCache cache,
    ILogger<GetAllDistinctDriverRequestHandler> logger
) : IRequestHandler<GetAllDistinctDriverRequest, Result<GetAllDistinctDriverResponse>>
{
    public async Task<Result<GetAllDistinctDriverResponse>> Handle(
        GetAllDistinctDriverRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Vehicle? vehicle = await repositoryVehicle.GetByIdDistinctAsync(request.VehicleId);

            if (vehicle is null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.VehicleId));
            }

            Guid tenantId = vehicle.GetTenantId();

            string cacheKey = $"driversDistinct?t={tenantId}";

            string? jsonString = await cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                GetAllDistinctDriverResponse? cachedResult = JsonSerializer.Deserialize<GetAllDistinctDriverResponse>(jsonString);

                if (cachedResult is not null)
                {
                    return Result.Ok(cachedResult);
                }
            }

            List<Driver> drivers = await repositoryDriver.GetAllByTenantDistinctAsync(tenantId);

            GetAllDistinctDriverResponse response = mapper.Map<GetAllDistinctDriverResponse>(drivers);

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
