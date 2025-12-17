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
    int FuelTankCapacity,
    int Year,
    string? Image,
    VehicleGroupDto Group,
    bool IsActive
);

public record VehicleGroupDto(
    Guid Id,
    string Name,
    bool IsActive
);