using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.RentalExtras;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LocadoraDeAutomoveis.Application.RentalExtras.Commands.GetAll;

public class GetAllRentalExtraRequestHandler(
    IMapper mapper,
    IRepositoryRentalExtra repositoryRentalExtra,
    IDistributedCache cache,
    ILogger<GetAllRentalExtraRequestHandler> logger
) : IRequestHandler<GetAllRentalExtraRequest, Result<GetAllRentalExtraResponse>>
{
    public async Task<Result<GetAllRentalExtraResponse>> Handle(
        GetAllRentalExtraRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<RentalExtra> extras = [];
            bool quantityProvided = request.Quantity.HasValue && request.Quantity.Value > 0;
            bool inactiveProvided = request.IsActive.HasValue;

            string cacheSubKey = quantityProvided ? $"qty-{request.Quantity!.Value}:" : "qty-all:";
            cacheSubKey += inactiveProvided ? $"active-{request.IsActive!.Value}" : "active-true";

            string cacheKey = $"extras:{cacheSubKey}";

            string? jsonString = await cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                GetAllRentalExtraResponse? cachedResult = JsonSerializer.Deserialize<GetAllRentalExtraResponse>(jsonString);

                if (cachedResult is not null)
                {
                    return Result.Ok(cachedResult);
                }
            }

            if (quantityProvided && inactiveProvided)
            {
                int quantity = request.Quantity!.Value;
                bool isActive = request.IsActive!.Value;

                extras = await repositoryRentalExtra.GetAllAsync(quantity, isActive);
            }
            else if (inactiveProvided)
            {
                bool isActive = request.IsActive!.Value;

                extras = await repositoryRentalExtra.GetAllAsync(isActive);
            }
            else if (quantityProvided)
            {
                int quantity = request.Quantity!.Value;

                extras = await repositoryRentalExtra.GetAllAsync(quantity);
            }
            else
            {
                extras = await repositoryRentalExtra.GetAllAsync(true);
            }

            GetAllRentalExtraResponse response = mapper.Map<GetAllRentalExtraResponse>(extras);

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
