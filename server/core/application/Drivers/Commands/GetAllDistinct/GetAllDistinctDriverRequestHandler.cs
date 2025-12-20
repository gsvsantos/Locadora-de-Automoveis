using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Drivers.Commands.GetAllDistinct;

public class GetAllDistinctDriverRequestHandler(
    IMapper mapper,
    IRepositoryDriver repositoryDriver,
    IRepositoryVehicle repositoryVehicle,
    ILogger<GetAllDistinctDriverRequestHandler> logger
) : IRequestHandler<GetAllDistinctDriverRequest, Result<GetAllDistinctDriverResponse>>
{
    public async Task<Result<GetAllDistinctDriverResponse>> Handle(
        GetAllDistinctDriverRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Vehicle? vehicle = await repositoryVehicle.GetByIdDistinctAsync(request.VehicleId);

            if (vehicle is null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.VehicleId));
            }

            Guid tenantId = vehicle.GetTenantId();

            List<Driver> drivers = await repositoryDriver.GetAllByTenantDistinctAsync(tenantId);

            GetAllDistinctDriverResponse response = mapper.Map<GetAllDistinctDriverResponse>(drivers);

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
