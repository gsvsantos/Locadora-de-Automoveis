using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Groups.Commands.GetById;

public record GetByIdGroupRequest(
    Guid Id
) : IRequest<Result<GetByIdGroupResponse>>;
