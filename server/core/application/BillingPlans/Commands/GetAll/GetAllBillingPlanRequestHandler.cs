using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.BillingPlans;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LocadoraDeAutomoveis.Application.BillingPlans.Commands.GetAll;

public class GetAllBillingPlanRequestHandler(
    IMapper mapper,
    IRepositoryBillingPlan repositoryBillingPlan,
    IDistributedCache cache,
    ILogger<GetAllBillingPlanRequestHandler> logger
) : IRequestHandler<GetAllBillingPlanRequest, Result<GetAllBillingPlanResponse>>
{
    public async Task<Result<GetAllBillingPlanResponse>> Handle(
        GetAllBillingPlanRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<BillingPlan> billingPlans = [];
            bool quantityProvided = request.Quantity.HasValue && request.Quantity.Value > 0;
            bool inactiveProvided = request.IsActive.HasValue;

            string versionKey = "billingPlans:master-version";
            string? version = await cache.GetStringAsync(versionKey, cancellationToken);

            if (string.IsNullOrEmpty(version))
            {
                version = Guid.NewGuid().ToString();

                await cache.SetStringAsync(versionKey, version, cancellationToken);
            }

            string cacheSubKey = quantityProvided ? $"qty={request.Quantity!.Value}:" : "qty=all:";
            cacheSubKey += inactiveProvided ? $":active={request.IsActive!.Value}" : ":active=true";

            string cacheKey = $"billingPlans:v={version}:{cacheSubKey}";

            string? jsonString = await cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                GetAllBillingPlanResponse? cachedResult = JsonSerializer.Deserialize<GetAllBillingPlanResponse>(jsonString);

                if (cachedResult is not null)
                {
                    return Result.Ok(cachedResult);
                }
            }

            if (quantityProvided && inactiveProvided)
            {
                int quantity = request.Quantity!.Value;
                bool isActive = request.IsActive!.Value;

                billingPlans = await repositoryBillingPlan.GetAllAsync(quantity, isActive);
            }
            else if (inactiveProvided)
            {
                bool isActive = request.IsActive!.Value;

                billingPlans = await repositoryBillingPlan.GetAllAsync(isActive);
            }
            else if (quantityProvided)
            {
                int quantity = request.Quantity!.Value;

                billingPlans = await repositoryBillingPlan.GetAllAsync(quantity);
            }
            else
            {
                billingPlans = await repositoryBillingPlan.GetAllAsync(true);
            }

            GetAllBillingPlanResponse response = mapper.Map<GetAllBillingPlanResponse>(billingPlans);

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
