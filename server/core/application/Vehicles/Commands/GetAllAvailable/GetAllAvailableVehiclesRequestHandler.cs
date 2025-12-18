using AutoMapper;
using FluentResults;
using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.Shared;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAllAvailable;

public class GetAllAvailableVehiclesRequestHandler(
    IRepositoryVehicle repositoryVehicle,
    IMapper mapper
) : IRequestHandler<GetAllAvailableVehiclesRequest, Result<PagedResult<VehicleDto>>>
{
    public async Task<Result<PagedResult<VehicleDto>>> Handle(
        GetAllAvailableVehiclesRequest request,
        CancellationToken cancellationToken)
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
}