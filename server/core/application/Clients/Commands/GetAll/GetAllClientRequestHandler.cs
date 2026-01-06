using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Clients;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.GetAll;

public class GetAllClientRequestHandler(
    IMapper mapper,
    IRepositoryClient repositoryClient,
    IDistributedCache cache,
    ILogger<GetAllClientRequestHandler> logger
) : IRequestHandler<GetAllClientRequest, Result<GetAllClientResponse>>
{
    public async Task<Result<GetAllClientResponse>> Handle(
        GetAllClientRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Client> clients = [];
            bool quantityProvided = request.Quantity.HasValue && request.Quantity.Value > 0;
            bool inactiveProvided = request.IsActive.HasValue;

            string versionKey = "clients:master-version";
            string? version = await cache.GetStringAsync(versionKey, cancellationToken);

            if (string.IsNullOrEmpty(version))
            {
                version = Guid.NewGuid().ToString();

                await cache.SetStringAsync(versionKey, version, cancellationToken);
            }

            string cacheSubKey = quantityProvided ? $"qty={request.Quantity!.Value}:" : "qty=all:";
            cacheSubKey += inactiveProvided ? $":active={request.IsActive!.Value}" : ":active=true";

            string cacheKey = $"clients:v={version}:{cacheSubKey}";

            string? jsonString = await cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                GetAllClientResponse? cachedResult = JsonSerializer.Deserialize<GetAllClientResponse>(jsonString);

                if (cachedResult is not null)
                {
                    return Result.Ok(cachedResult);
                }
            }

            if (quantityProvided && inactiveProvided)
            {
                int quantity = request.Quantity!.Value;
                bool isActive = request.IsActive!.Value;

                clients = await repositoryClient.GetAllAsync(quantity, isActive);
            }
            else if (inactiveProvided)
            {
                bool isActive = request.IsActive!.Value;

                clients = await repositoryClient.GetAllAsync(isActive);
            }
            else if (quantityProvided)
            {
                int quantity = request.Quantity!.Value;

                clients = await repositoryClient.GetAllAsync(quantity);
            }
            else
            {
                clients = await repositoryClient.GetAllAsync(true);
            }

            GetAllClientResponse response = mapper.Map<GetAllClientResponse>(clients);

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
