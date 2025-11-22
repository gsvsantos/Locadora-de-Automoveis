using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Groups.Commands.Update;

public record UpdateGroupRequestPartial(string Name
) : IRequest<Result<UpdateGroupResponse>>;

public record UpdateGroupRequest(
    Guid Id,
    string Name
) : IRequest<Result<UpdateGroupResponse>>;