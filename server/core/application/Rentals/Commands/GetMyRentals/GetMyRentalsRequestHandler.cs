using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetMyRentals;

public class GetMyRentalsRequestHandler(
    IMapper mapper,
    IRepositoryRental repositoryRental,
    IDistributedCache cache,
    IUserContext userContext,
    ILogger<GetMyRentalsRequestHandler> logger
) : IRequestHandler<GetMyRentalsRequest, Result<GetMyRentalsResponse>>
{
    public async Task<Result<GetMyRentalsResponse>> Handle(
        GetMyRentalsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Guid loginUserId = userContext.GetUserId();

            if (request.PageNumber < 1)
            {
                return Result.Fail(ErrorResults.BadRequestError("PageNumber must be greater than or equal to 1."));
            }

            if (request.PageSize is < 1 or > 100)
            {
                return Result.Fail(ErrorResults.BadRequestError("PageSize must be between 1 and 100."));
            }

            string versionKey = "rentals:master-version";
            string? version = await cache.GetStringAsync(versionKey, cancellationToken);

            if (string.IsNullOrEmpty(version))
            {
                version = Guid.NewGuid().ToString();

                await cache.SetStringAsync(versionKey, version, cancellationToken);
            }

            string termKey = string.IsNullOrWhiteSpace(request.Term) ? "all" : request.Term.Trim().ToLower();
            string tenantKey = request.TenantId.HasValue ? request.TenantId.Value.ToString() : "all";
            string statusKey = request.Status.HasValue ? request.Status.Value.ToString() : "all";

            string cacheSubKey = $"uid={loginUserId}:p={request.PageNumber}:s={request.PageSize}:t={termKey}:tid={tenantKey}:st={statusKey}";

            string cacheKey = $"rentals:v={version}:{cacheSubKey}";

            string? jsonString = await cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                GetMyRentalsResponse? cachedResult = JsonSerializer.Deserialize<GetMyRentalsResponse>(jsonString);

                if (cachedResult is not null)
                {
                    return Result.Ok(cachedResult);
                }
            }

            PagedResult<Rental> pagedRentals = await repositoryRental.GetMyRentalsDistinctAsync(
                loginUserId,
                request.PageNumber,
                request.PageSize,
                request.Term,
                request.TenantId,
                request.Status,
                cancellationToken
            );

            List<ClientRentalDto> rentalDtos = mapper.Map<List<ClientRentalDto>>(pagedRentals.Items);

            PagedResult<ClientRentalDto> result = new(
                rentalDtos,
                pagedRentals.TotalCount,
                pagedRentals.CurrentPage,
                pagedRentals.PageSize
            );

            GetMyRentalsResponse response = new(result);

            string jsonPayload = JsonSerializer.Serialize(response);

            DistributedCacheEntryOptions cacheOptions = new()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3)
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
