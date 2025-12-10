using FluentResults;
using LocadoraDeAutomoveis.Domain.Rentals;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.Return;

public record ReturnRentalRequestPartial(
    decimal EndKm,
    EFuelLevel FuelLevel
);

public record ReturnRentalRequest(
    Guid Id,
    decimal EndKm,
    EFuelLevel FuelLevel
) : IRequest<Result<ReturnRentalResponse>>;
