using FluentResults;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.Update;

public record UpdateVehicleRequestPartial(
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

public record UpdateVehicleRequest(
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
) : IRequest<Result<UpdateVehicleResponse>>;
