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
