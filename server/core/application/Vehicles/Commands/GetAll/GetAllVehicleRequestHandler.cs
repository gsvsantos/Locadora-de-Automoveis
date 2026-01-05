using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAll;

public class GetAllVehicleRequestHandler(
    IMapper mapper,
    IRepositoryVehicle repositoryVehicle,
    IDistributedCache cache,
    ILogger<GetAllVehicleRequestHandler> logger
) : IRequestHandler<GetAllVehicleRequest, Result<GetAllVehicleResponse>>
{
    public async Task<Result<GetAllVehicleResponse>> Handle(
        GetAllVehicleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Vehicle> vehicles = [];
            bool quantityProvided = request.Quantity.HasValue && request.Quantity.Value > 0;
            bool inactiveProvided = request.IsActive.HasValue;

            string cacheSubKey = quantityProvided ? $"qty-{request.Quantity!.Value}:" : "qty-all:";
            cacheSubKey += inactiveProvided ? $"active-{request.IsActive!.Value}" : "active-true";

            string cacheKey = $"vehicles:{cacheSubKey}";

            string? jsonString = await cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                GetAllVehicleResponse? cachedResult = JsonSerializer.Deserialize<GetAllVehicleResponse>(jsonString);

                if (cachedResult is not null)
                {
                    return Result.Ok(cachedResult);
                }
            }

            if (quantityProvided && inactiveProvided)
            {
                int quantity = request.Quantity!.Value;
                bool isActive = request.IsActive!.Value;

                vehicles = await repositoryVehicle.GetAllAsync(quantity, isActive);
            }
            else if (inactiveProvided)
            {
                bool isActive = request.IsActive!.Value;

                vehicles = await repositoryVehicle.GetAllAsync(isActive);
            }
            else if (quantityProvided)
            {
                int quantity = request.Quantity!.Value;

                vehicles = await repositoryVehicle.GetAllAsync(quantity);
            }
            else
            {
                vehicles = await repositoryVehicle.GetAllAsync(true);
            }

            GetAllVehicleResponse response = mapper.Map<GetAllVehicleResponse>(vehicles);

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
