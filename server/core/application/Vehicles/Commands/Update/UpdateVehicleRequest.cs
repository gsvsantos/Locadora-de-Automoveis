using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.Update;

public record UpdateVehicleRequestPartial(
    string LicensePlate,
    string Brand,
    string Color,
    string Model,
    string FuelType,
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
    string FuelType,
    int CapacityInLiters,
    int Year,
    string? PhotoPath,
    Guid GroupId
) : IRequest<Result<UpdateVehicleResponse>>;
