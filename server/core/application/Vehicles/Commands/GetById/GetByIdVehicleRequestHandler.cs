using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.GetById;

public class GetByIdVehicleRequestHandler(
    IRepositoryVehicle repositoryVehicle,
    ILogger<GetByIdVehicleRequestHandler> logger
) : IRequestHandler<GetByIdVehicleRequest, Result<GetByIdVehicleResponse>>
{
    public async Task<Result<GetByIdVehicleResponse>> Handle(
        GetByIdVehicleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Vehicle? selectedVehicle = await repositoryVehicle.GetByIdAsync(request.Id);

            if (selectedVehicle is null)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.Id));
            }

            GetByIdVehicleResponse response = new(
                new VehicleDto(
                    selectedVehicle.Id,
                    selectedVehicle.LicensePlate,
                    selectedVehicle.Brand,
                    selectedVehicle.Color,
                    selectedVehicle.Model,
                    selectedVehicle.FuelType,
                    selectedVehicle.CapacityInLiters,
                    selectedVehicle.Year,
                    selectedVehicle.PhotoPath,
                    selectedVehicle.GroupId
                )
            );

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error retrieving vehicle by ID {VehicleId}", request.Id);

            return Result.Fail(new Error("An error occurred while retrieving the vehicle."));
        }
    }
}
