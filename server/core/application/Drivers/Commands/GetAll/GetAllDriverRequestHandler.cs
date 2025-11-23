using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Drivers;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Drivers.Commands.GetAll;

public class GetAllDriverRequestHandler(
    IRepositoryDriver repositoryDriver,
    ILogger<GetAllDriverRequestHandler> logger
) : IRequestHandler<GetAllDriverRequest, Result<GetAllDriverResponse>>
{
    public async Task<Result<GetAllDriverResponse>> Handle(
        GetAllDriverRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Driver> drivers =
                request.Quantity.HasValue && request.Quantity.Value > 0
                ? await repositoryDriver.GetAllAsync(request.Quantity.Value)
                : await repositoryDriver.GetAllAsync();

            GetAllDriverResponse response = new(
                drivers.Count,
                drivers.Select(driver => new DriverDto(
                    driver.Id,
                    driver.FullName,
                    driver.Email,
                    driver.PhoneNumber,
                    driver.Document,
                    driver.LicenseNumber,
                    driver.LicenseValidity,
                    driver.ClientCPF.FullName,
                    driver.ClientCNPJ?.FullName
                )).ToImmutableList()
            );

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
