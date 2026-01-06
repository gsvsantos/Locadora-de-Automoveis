using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Clients;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.GetIndividuals;

public class GetIndividualsRequestHandler(
    IMapper mapper,
    IRepositoryClient repositoryClient,
    IDistributedCache cache,
    ILogger<GetIndividualsRequestHandler> logger
) : IRequestHandler<GetIndividualsRequest, Result<GetIndividualsResponse>>
{
    public async Task<Result<GetIndividualsResponse>> Handle(
        GetIndividualsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Client? selectedClient = await repositoryClient.GetByIdAsync(request.Id);

            if (selectedClient is null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.Id));
            }

            if (selectedClient.Type != EClientType.Business)
            {
                return Result.Fail(ClientErrorResults.ClientIsNotBusinessError(selectedClient.FullName));
            }

            string versionKey = "clients:master-version";
            string? version = await cache.GetStringAsync(versionKey, cancellationToken);

            if (string.IsNullOrEmpty(version))
            {
                version = Guid.NewGuid().ToString();

                await cache.SetStringAsync(versionKey, version, cancellationToken);
            }

            string cacheKey = $"clients:v={version}:b={selectedClient.Id}";

            string? jsonString = await cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                GetIndividualsResponse? cachedResult = JsonSerializer.Deserialize<GetIndividualsResponse>(jsonString);

                if (cachedResult is not null)
                {
                    return Result.Ok(cachedResult);
                }
            }

            List<Client> clients = await repositoryClient.GetIndividualClientsFromBusinessId(selectedClient.Id, cancellationToken);

            GetIndividualsResponse response = mapper.Map<GetIndividualsResponse>(clients);

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
