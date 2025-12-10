using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAll;

public class GetAllVehicleRequestHandler(
    IMapper mapper,
    IRepositoryVehicle repositoryVehicle,
    ILogger<GetAllVehicleRequestHandler> logger
) : IRequestHandler<GetAllVehicleRequest, Result<GetAllVehicleResponse>>
{
    public async Task<Result<GetAllVehicleResponse>> Handle(
        GetAllVehicleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<Vehicle> vehicles = [];
            bool quantityProvided = request.Quantity.HasValue && request.Quantity.Value > 0;
            bool inactiveProvided = request.IsActive.HasValue;

            if (quantityProvided && inactiveProvided)
            {
                int quantity = request.Quantity!.Value;
                bool isActive = request.IsActive!.Value;

                vehicles = await repositoryVehicle.GetAllAsync(quantity, isActive);
            }
            else if (inactiveProvided)
            {
                bool isActive = request.IsActive!.Value;

                vehicles = await repositoryVehicle.GetAllAsync(isActive);
            }
            else if (quantityProvided)
            {
                int quantity = request.Quantity!.Value;

                vehicles = await repositoryVehicle.GetAllAsync(quantity);
            }
            else
            {
                vehicles = await repositoryVehicle.GetAllAsync(true);
            }

            GetAllVehicleResponse response = mapper.Map<GetAllVehicleResponse>(vehicles);

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
