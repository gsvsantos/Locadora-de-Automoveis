using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Drivers.Commands.Delete;

public record DeleteDriverRequest(
    Guid Id
) : IRequest<Result<DeleteDriverResponse>>;
