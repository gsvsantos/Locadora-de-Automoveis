using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAll;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAllAvailable;

public record GetAllAvailableVehiclesResponse(
    PagedResult<VehicleDto> Vehicles
);