using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.GetById;

public record GetByIdClientRequest(
    Guid Id
) : IRequest<Result<GetByIdClientResponse>>;
