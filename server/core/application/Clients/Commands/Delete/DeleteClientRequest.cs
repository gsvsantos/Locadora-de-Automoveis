using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.Delete;

public record DeleteClientRequest(
    Guid Id
) : IRequest<Result<DeleteClientResponse>>;
