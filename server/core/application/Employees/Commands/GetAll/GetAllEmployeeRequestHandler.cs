using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Employees;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LocadoraDeAutomoveis.Application.Employees.Commands.GetAll;

public class GetAllEmployeeRequestHandler(
    IMapper mapper,
    IRepositoryEmployee repositoryEmployee,
    IDistributedCache cache,
    ILogger<GetAllEmployeeRequestHandler> logger
) : IRequestHandler<GetAllEmployeeRequest, Result<GetAllEmployeeResponse>>
{
    public async Task<Result<GetAllEmployeeResponse>> Handle(
        GetAllEmployeeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Employee> employees = [];
            bool quantityProvided = request.Quantity.HasValue && request.Quantity.Value > 0;
            bool inactiveProvided = request.IsActive.HasValue;

            string cacheSubKey = quantityProvided ? $"qty-{request.Quantity!.Value}:" : "qty-all:";
            cacheSubKey += inactiveProvided ? $"active-{request.IsActive!.Value}" : "active-true";

            string cacheKey = $"employees:{cacheSubKey}";

            string? jsonString = await cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                GetAllEmployeeResponse? cachedResult = JsonSerializer.Deserialize<GetAllEmployeeResponse>(jsonString);

                if (cachedResult is not null)
                {
                    return Result.Ok(cachedResult);
                }
            }

            if (quantityProvided && inactiveProvided)
            {
                int quantity = request.Quantity!.Value;
                bool isActive = request.IsActive!.Value;

                employees = await repositoryEmployee.GetAllAsync(quantity, isActive);
            }
            else if (inactiveProvided)
            {
                bool isActive = request.IsActive!.Value;

                employees = await repositoryEmployee.GetAllAsync(isActive);
            }
            else if (quantityProvided)
            {
                int quantity = request.Quantity!.Value;

                employees = await repositoryEmployee.GetAllAsync(quantity);
            }
            else
            {
                employees = await repositoryEmployee.GetAllAsync(true);
            }

            GetAllEmployeeResponse response = mapper.Map<GetAllEmployeeResponse>(employees);

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
