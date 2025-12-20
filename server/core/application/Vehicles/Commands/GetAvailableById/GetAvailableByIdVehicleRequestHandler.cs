using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Rentals;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAvailableById;

public class GetAvailableByIdVehicleRequestHandler(
    IMapper mapper,
    IRepositoryVehicle repositoryVehicle,
    IRepositoryRental repositoryRental,
    ILogger<GetAvailableByIdVehicleRequestHandler> logger
) : IRequestHandler<GetAvailableByIdVehicleRequest, Result<GetAvailableByIdVehicleResponse>>
{
    public async Task<Result<GetAvailableByIdVehicleResponse>> Handle(
        GetAvailableByIdVehicleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Vehicle? selectedVehicle = await repositoryVehicle.GetByIdDistinctAsync(request.Id);

            if (selectedVehicle is null || !selectedVehicle.IsActive)
            {
                return Result.Fail(ErrorResults.NotFoundError(request.Id));
            }

            bool isRented = await repositoryRental.HasActiveRentalsByVehicle(selectedVehicle.Id);

            if (isRented)
            {
                return Result.Fail(VehicleErrorResults.VehicleUnavailable(request.Id));
            }

            GetAvailableByIdVehicleResponse response = mapper.Map<GetAvailableByIdVehicleResponse>(selectedVehicle);

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error retrieving available vehicle {VehicleId}", request.Id
            );

            return Result.Fail(new Error("An error occurred while retrieving the vehicle."));
        }
    }
}