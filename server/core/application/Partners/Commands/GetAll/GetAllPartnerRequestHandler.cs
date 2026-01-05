using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Partners;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LocadoraDeAutomoveis.Application.Partners.Commands.GetAll;

public class GetAllPartnerRequestHandler(
    IMapper mapper,
    IRepositoryPartner repositoryPartner,
    IDistributedCache cache,
    ILogger<GetAllPartnerRequestHandler> logger
) : IRequestHandler<GetAllPartnerRequest, Result<GetAllPartnerResponse>>
{
    public async Task<Result<GetAllPartnerResponse>> Handle(
        GetAllPartnerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Partner> partners = [];
            bool quantityProvided = request.Quantity.HasValue && request.Quantity.Value > 0;
            bool inactiveProvided = request.IsActive.HasValue;

            string cacheSubKey = quantityProvided ? $"qty-{request.Quantity!.Value}:" : "qty-all:";
            cacheSubKey += inactiveProvided ? $"active-{request.IsActive!.Value}" : "active-true";

            string cacheKey = $"partners:{cacheSubKey}";

            string? jsonString = await cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                GetAllPartnerResponse? cachedResult = JsonSerializer.Deserialize<GetAllPartnerResponse>(jsonString);

                if (cachedResult is not null)
                {
                    return Result.Ok(cachedResult);
                }
            }

            if (quantityProvided && inactiveProvided)
            {
                int quantity = request.Quantity!.Value;
                bool isActive = request.IsActive!.Value;

                partners = await repositoryPartner.GetAllAsync(quantity, isActive);
            }
            else if (inactiveProvided)
            {
                bool isActive = request.IsActive!.Value;

                partners = await repositoryPartner.GetAllAsync(isActive);
            }
            else if (quantityProvided)
            {
                int quantity = request.Quantity!.Value;

                partners = await repositoryPartner.GetAllAsync(quantity);
            }
            else
            {
                partners = await repositoryPartner.GetAllAsync(true);
            }

            GetAllPartnerResponse response = mapper.Map<GetAllPartnerResponse>(partners);

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
