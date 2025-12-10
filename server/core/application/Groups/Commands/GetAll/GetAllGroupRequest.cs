using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Groups.Commands.GetAll;

public record GetAllGroupRequestPartial(
    int? Quantity,
    bool? IsActive
);

public record GetAllGroupRequest(
    int? Quantity,
    bool? IsActive
) : IRequest<Result<GetAllGroupResponse>>;
