using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.RentalExtras.Commands.GetById;

public record GetByIdRentalExtraRequest(
    Guid Id
) : IRequest<Result<GetByIdRentalExtraResponse>>;
