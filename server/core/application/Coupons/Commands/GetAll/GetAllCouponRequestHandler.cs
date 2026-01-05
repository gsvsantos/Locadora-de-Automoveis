using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Coupons;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LocadoraDeAutomoveis.Application.Coupons.Commands.GetAll;

public class GetAllCouponRequestHandler(
    IMapper mapper,
    IRepositoryCoupon repositoryCoupon,
    IDistributedCache cache,
    ILogger<GetAllCouponRequestHandler> logger
) : IRequestHandler<GetAllCouponRequest, Result<GetAllCouponResponse>>
{
    public async Task<Result<GetAllCouponResponse>> Handle(
        GetAllCouponRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Coupon> coupons = [];
            bool quantityProvided = request.Quantity.HasValue && request.Quantity.Value > 0;
            bool inactiveProvided = request.IsActive.HasValue;

            string cacheSubKey = quantityProvided ? $"qty-{request.Quantity!.Value}:" : "qty-all:";
            cacheSubKey += inactiveProvided ? $"active-{request.IsActive!.Value}" : "active-true";

            string cacheKey = $"coupons:{cacheSubKey}";

            string? jsonString = await cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                GetAllCouponResponse? cachedResult = JsonSerializer.Deserialize<GetAllCouponResponse>(jsonString);

                if (cachedResult is not null)
                {
                    return Result.Ok(cachedResult);
                }
            }

            if (quantityProvided && inactiveProvided)
            {
                int quantity = request.Quantity!.Value;
                bool isActive = request.IsActive!.Value;

                coupons = await repositoryCoupon.GetAllAsync(quantity, isActive);
            }
            else if (inactiveProvided)
            {
                bool isActive = request.IsActive!.Value;

                coupons = await repositoryCoupon.GetAllAsync(isActive);
            }
            else if (quantityProvided)
            {
                int quantity = request.Quantity!.Value;

                coupons = await repositoryCoupon.GetAllAsync(quantity);
            }
            else
            {
                coupons = await repositoryCoupon.GetAllAsync(true);
            }

            GetAllCouponResponse response = mapper.Map<GetAllCouponResponse>(coupons);

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