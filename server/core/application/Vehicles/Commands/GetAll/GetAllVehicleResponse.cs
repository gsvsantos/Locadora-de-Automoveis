using LocadoraDeAutomoveis.Domain.Vehicles;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.GetAll;

public record GetAllVehicleResponse(
    int Quantity,
    ImmutableList<VehicleDto> Vehicles
);

public record VehicleDto(
    Guid Id,
    string LicensePlate,
    string Brand,
    string Color,
    string Model,
    EFuelType FuelType,
    int CapacityInLiters,
    int Year,
    string? PhotoPath,
    Guid GroupId
);