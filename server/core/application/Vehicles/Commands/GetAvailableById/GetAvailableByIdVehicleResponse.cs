using LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAll;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAvailableById;

public record GetAvailableByIdVehicleResponse(
    VehicleDto Vehicle
);