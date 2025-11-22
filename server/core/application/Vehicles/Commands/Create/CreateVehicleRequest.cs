using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Vehicles.Commands.Create;

public record CreateVehicleRequest(
    string LicensePlate,
    string Brand,
    string Color,
    string Model,
    string FuelType,
    int CapacityInLiters,
    DateTimeOffset Year,
    string? PhotoPath,
    Guid GroupId
) : IRequest<Result<CreateVehicleResponse>>;
