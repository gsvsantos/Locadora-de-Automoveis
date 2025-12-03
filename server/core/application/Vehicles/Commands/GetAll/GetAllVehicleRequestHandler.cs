using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAll;

public class GetAllVehicleRequestHandler(
    IRepositoryVehicle repositoryVehicle,
    ILogger<GetAllVehicleRequestHandler> logger
) : IRequestHandler<GetAllVehicleRequest, Result<GetAllVehicleResponse>>
{
    public async Task<Result<GetAllVehicleResponse>> Handle(
        GetAllVehicleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Vehicle> vehicles;

            if (request.GroupId.HasValue && request.GroupId != Guid.Empty)
            {
                vehicles = await repositoryVehicle.GetByGroupIdAsync(request.GroupId.Value);
            }
            else if (request.Quantity.HasValue && request.Quantity.Value > 0)
            {
                vehicles = await repositoryVehicle.GetAllAsync(request.Quantity.Value);
            }
            else
            {
                vehicles = await repositoryVehicle.GetAllAsync();
            }

            GetAllVehicleResponse response = new(
                vehicles.Count,
                vehicles.Select(vehicle => new VehicleDto(
                    vehicle.Id,
                    vehicle.LicensePlate,
                    vehicle.Brand,
                    vehicle.Color,
                    vehicle.Model,
                    vehicle.FuelType,
                    vehicle.FuelTankCapacity,
                    vehicle.Year,
                    vehicle.PhotoPath,
                    vehicle.GroupId
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
