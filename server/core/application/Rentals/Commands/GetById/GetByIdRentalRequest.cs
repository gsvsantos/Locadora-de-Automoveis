using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Rentals.Commands.GetById;

public record GetByIdRentalRequest(
    Guid Id
) : IRequest<Result<GetByIdRentalResponse>>;