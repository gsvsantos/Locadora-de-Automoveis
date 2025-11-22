using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Groups.Commands.Delete;

public record DeleteGroupRequest(
    Guid Id
) : IRequest<Result<DeleteGroupResponse>>;
