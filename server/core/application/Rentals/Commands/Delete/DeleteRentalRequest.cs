using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.Delete;

public record DeleteRentalRequest(
    Guid Id
) : IRequest<Result<DeleteRentalResponse>>;
