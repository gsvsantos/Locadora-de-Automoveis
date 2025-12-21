using FluentResults;
using LocadoraDeAutomoveis.Domain.Rentals;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.Return;

public record ReturnRentalRequestPartial(
    decimal EndKm,
    EFuelLevel FuelLevelAtReturn
);

public record ReturnRentalRequest(
    Guid Id,
    decimal EndKm,
    EFuelLevel FuelLevelAtReturn
) : IRequest<Result<ReturnRentalResponse>>;
