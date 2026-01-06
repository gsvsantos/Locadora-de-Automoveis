using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Groups;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LocadoraDeAutomoveis.Application.Groups.Commands.GetAllDistinct;

public class GetAllDistinctGroupRequestHandler(
    IMapper mapper,
    IRepositoryGroup repositoryGroup,
    IDistributedCache cache,
    ILogger<GetAllDistinctGroupRequestHandler> logger
) : IRequestHandler<GetAllDistinctGroupRequest, Result<GetAllDistinctGroupResponse>>
{
    public async Task<Result<GetAllDistinctGroupResponse>> Handle(
        GetAllDistinctGroupRequest request, CancellationToken cancellationToken)
    {
        try
        {
            string versionKey = "groups:master-version";
            string? version = await cache.GetStringAsync(versionKey, cancellationToken);

            if (string.IsNullOrEmpty(version))
            {
                version = Guid.NewGuid().ToString();

                await cache.SetStringAsync(versionKey, version, cancellationToken);
            }

            string cacheKey = $"groups:v={version}:distinct";

            string? jsonString = await cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                GetAllDistinctGroupResponse? cachedResult = JsonSerializer.Deserialize<GetAllDistinctGroupResponse>(jsonString);

                if (cachedResult is not null)
                {
                    return Result.Ok(cachedResult);
                }
            }

            List<Group> groups = await repositoryGroup.GetAllDistinct();

            List<Group> distinctGroups = groups
                .GroupBy(g => g.Name.ToUpper().Trim())
                .Select(g => g.First())
                .ToList();

            GetAllDistinctGroupResponse response = mapper.Map<GetAllDistinctGroupResponse>(distinctGroups);

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