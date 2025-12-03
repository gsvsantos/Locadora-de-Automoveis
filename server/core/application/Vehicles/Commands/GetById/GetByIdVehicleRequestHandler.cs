using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.GetById;

public class GetByIdVehicleRequestHandler(
    IMapper mapper,
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

            GetByIdVehicleResponse response = mapper.Map<GetByIdVehicleResponse>(selectedVehicle);

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
