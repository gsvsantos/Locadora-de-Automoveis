using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Drivers.Commands.GetById;

public record GetByIdDriverRequest(
    Guid Id
) : IRequest<Result<GetByIdDriverResponse>>;
