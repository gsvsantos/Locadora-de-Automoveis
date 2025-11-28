using FluentResults;
using LocadoraDeAutomoveis.Domain.Vehicles;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.Create;

public record CreateVehicleRequest(
    string LicensePlate,
    string Brand,
    string Color,
    string Model,
    EFuelType FuelType,
    int CapacityInLiters,
    int Year,
    string? PhotoPath,
    Guid GroupId
) : IRequest<Result<CreateVehicleResponse>>;
