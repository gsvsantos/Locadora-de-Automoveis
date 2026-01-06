using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Rentals;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetAll;

public class GetAllRentalRequestHandler(
    IMapper mapper,
    IRepositoryRental repositoryRental,
    IDistributedCache cache,
    ILogger<GetAllRentalRequestHandler> logger
) : IRequestHandler<GetAllRentalRequest, Result<GetAllRentalResponse>>
{
    public async Task<Result<GetAllRentalResponse>> Handle(
        GetAllRentalRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Rental> rentals = [];
            bool quantityProvided = request.Quantity.HasValue && request.Quantity.Value > 0;
            bool inactiveProvided = request.IsActive.HasValue;

            string versionKey = "rentals:master-version";
            string? version = await cache.GetStringAsync(versionKey, cancellationToken);

            if (string.IsNullOrEmpty(version))
            {
                version = Guid.NewGuid().ToString();

                await cache.SetStringAsync(versionKey, version, cancellationToken);
            }

            string cacheSubKey = quantityProvided ? $"qty={request.Quantity!.Value}:" : "qty=all:";
            cacheSubKey += inactiveProvided ? $":active={request.IsActive!.Value}" : ":active=true";

            string cacheKey = $"rentals:v={version}:{cacheSubKey}";

            string? jsonString = await cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                GetAllRentalResponse? cachedResult = JsonSerializer.Deserialize<GetAllRentalResponse>(jsonString);

                if (cachedResult is not null)
                {
                    return Result.Ok(cachedResult);
                }
            }

            if (quantityProvided && inactiveProvided)
            {
                int quantity = request.Quantity!.Value;
                bool isActive = request.IsActive!.Value;

                rentals = await repositoryRental.GetAllAsync(quantity, isActive);
            }
            else if (inactiveProvided)
            {
                bool isActive = request.IsActive!.Value;

                rentals = await repositoryRental.GetAllAsync(isActive);
            }
            else if (quantityProvided)
            {
                int quantity = request.Quantity!.Value;

                rentals = await repositoryRental.GetAllAsync(quantity);
            }
            else
            {
                rentals = await repositoryRental.GetAllAsync(true);
            }

            GetAllRentalResponse response = mapper.Map<GetAllRentalResponse>(rentals);

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
