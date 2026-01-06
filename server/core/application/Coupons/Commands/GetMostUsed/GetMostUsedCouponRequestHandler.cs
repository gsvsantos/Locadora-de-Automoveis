using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.GetMostUsed;

public class GetMostUsedCouponRequestHandler(
    ICouponQueryService couponQueryService,
    IDistributedCache cache,
    IMapper mapper,
    ILogger<GetMostUsedCouponRequestHandler> logger
) : IRequestHandler<GetMostUsedCouponRequest, Result<GetMostUsedCouponResponse>>
{
    public async Task<Result<GetMostUsedCouponResponse>> Handle(
        GetMostUsedCouponRequest request, CancellationToken cancellationToken)
    {
        try
        {
            string versionKey = "coupons:master-version";
            string? version = await cache.GetStringAsync(versionKey, cancellationToken);

            if (string.IsNullOrEmpty(version))
            {
                version = Guid.NewGuid().ToString();
                await cache.SetStringAsync(versionKey, version, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
                }, cancellationToken);
            }

            string cacheKey = $"coupons:v={version}:most-used";

            string? jsonString = await cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                GetMostUsedCouponResponse? cachedResult = JsonSerializer.Deserialize<GetMostUsedCouponResponse>(jsonString);
                if (cachedResult is not null)
                {
                    return Result.Ok(cachedResult);
                }
            }

            List<CouponUsageDto> coupons = await couponQueryService.GetMostUsedCouponsAsync();

            GetMostUsedCouponResponse response = mapper.Map<GetMostUsedCouponResponse>(coupons);

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
                ex, "Failed to get most used coupons."
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
