using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.Drivers;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAllAvailable;

public class GetAllAvailableVehiclesRequestHandler(
    IMapper mapper,
    IRepositoryVehicle repositoryVehicle,
    IRepositoryDriver repositoryDriver,
    IRepositoryRental repositoryRental,
    IDistributedCache cache,
    ILogger<GetAllAvailableVehiclesRequestHandler> logger
) : IRequestHandler<GetAllAvailableVehiclesRequest, Result<GetAllAvailableVehiclesResponse>>
{
    public async Task<Result<GetAllAvailableVehiclesResponse>> Handle(
        GetAllAvailableVehiclesRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Guid> rentedIds = await repositoryRental.GetRentedVehicleIds();

            List<Guid?> tenantsWithDrivers = await repositoryDriver.GetTenantsWithActiveDriversAsync();

            PagedResult<Vehicle> pagedVehicles = await repositoryVehicle.GetAllAvailableAsync(
            request.PageNumber,
            request.PageSize,
            request.Term,
            request.GroupId,
            request.FuelType,
            rentedIds,
            tenantsWithDrivers,
            cancellationToken
            );

            List<VehicleDto> vehicleDtos = mapper.Map<List<VehicleDto>>(pagedVehicles.Items);

            PagedResult<VehicleDto> result = new(
                vehicleDtos,
                pagedVehicles.TotalCount,
                request.PageNumber,
                request.PageSize
            );

            GetAllAvailableVehiclesResponse response = new(result);

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