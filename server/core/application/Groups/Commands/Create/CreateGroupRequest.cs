using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Groups.Commands.Create;

public record CreateGroupRequest(string Name)
    : IRequest<Result<CreateGroupResponse>>;
