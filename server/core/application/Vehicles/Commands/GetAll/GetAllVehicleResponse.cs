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
    string FuelType,
    int CapacityInLiters,
    DateTimeOffset Year,
    string? PhotoPath,
    Guid GroupId
);