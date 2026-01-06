using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAllAvailable;

public class GetAllAvailableVehiclesRequestHandler(
    IMapper mapper,
    IRepositoryVehicle repositoryVehicle,
    IRepositoryDriver repositoryDriver,
    IRepositoryRental repositoryRental,
    IDistributedCache cache,
    ILogger<GetAllAvailableVehiclesRequestHandler> logger
) : IRequestHandler<GetAllAvailableVehiclesRequest, Result<GetAllAvailableVehiclesResponse>>
{
    public async Task<Result<GetAllAvailableVehiclesResponse>> Handle(
        GetAllAvailableVehiclesRequest request, CancellationToken cancellationToken)
    {
        try
        {
            string versionKey = "vehicles:master-version";
            string? version = await cache.GetStringAsync(versionKey, cancellationToken);

            if (string.IsNullOrEmpty(version))
            {
                version = Guid.NewGuid().ToString();

                await cache.SetStringAsync(versionKey, version, cancellationToken);
            }

            string termKey = string.IsNullOrWhiteSpace(request.Term) ? "all" : request.Term.Trim().ToLower();
            string groupKey = request.GroupId.HasValue ? request.GroupId.Value.ToString() : "all";
            string fuelKey = request.FuelType.HasValue ? request.FuelType.Value.ToString() : "all";

            string cacheSubKey = $"p={request.PageNumber}:s={request.PageSize}:t={termKey}:g={groupKey}:f={fuelKey}";

            string cacheKey = $"vehicles:v={version}:{cacheSubKey}";

            string? jsonString = await cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                GetAllAvailableVehiclesResponse? cachedResult = JsonSerializer.Deserialize<GetAllAvailableVehiclesResponse>(jsonString);
                if (cachedResult is not null)
                {
                    return Result.Ok(cachedResult);
                }
            }

            List<Guid> rentedIds = await repositoryRental.GetRentedVehicleIds();

            List<Guid?> tenantsWithDrivers = await repositoryDriver.GetTenantsWithActiveDriversAsync();

            PagedResult<Vehicle> pagedVehicles = await repositoryVehicle.GetAllAvailableAsync(
            request.PageNumber,
            request.PageSize,
            request.Term,
            request.GroupId,
            request.FuelType,
            rentedIds,
            tenantsWithDrivers,
            cancellationToken
            );

            List<VehicleDto> vehicleDtos = mapper.Map<List<VehicleDto>>(pagedVehicles.Items);

            PagedResult<VehicleDto> result = new(
                vehicleDtos,
                pagedVehicles.TotalCount,
                request.PageNumber,
                request.PageSize
            );

            GetAllAvailableVehiclesResponse response = new(result);

            string jsonPayload = JsonSerializer.Serialize(response);

            DistributedCacheEntryOptions cacheOptions = new()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3)
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