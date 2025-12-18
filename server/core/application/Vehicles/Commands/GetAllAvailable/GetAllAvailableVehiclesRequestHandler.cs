using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAllAvailable;

public class GetAllAvailableVehiclesRequestHandler(
    IRepositoryVehicle repositoryVehicle,
    IMapper mapper,
    ILogger<GetAllAvailableVehiclesRequestHandler> logger
) : IRequestHandler<GetAllAvailableVehiclesRequest, Result<PagedResult<VehicleDto>>>
{
    public async Task<Result<PagedResult<VehicleDto>>> Handle(
        GetAllAvailableVehiclesRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            PagedResult<Vehicle> pagedVehicles = await repositoryVehicle.GetAllAvailableAsync(
            request.PageNumber,
            request.PageSize,
            request.Term,
            request.GroupId,
            request.FuelType,
            cancellationToken
            );

            List<VehicleDto> vehicleDtos = mapper.Map<List<VehicleDto>>(pagedVehicles.Items);

            PagedResult<VehicleDto> result = new(
                vehicleDtos,
                pagedVehicles.TotalCount,
                request.PageNumber,
                request.PageSize
            );

            return Result.Ok(result);
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