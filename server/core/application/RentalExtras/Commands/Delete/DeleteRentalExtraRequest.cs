using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.RentalExtras.Commands.Delete;

public record DeleteRentalExtraRequest(
    Guid Id
) : IRequest<Result<DeleteRentalExtraResponse>>;
