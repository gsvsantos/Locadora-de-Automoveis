using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Groups.Commands.GetAll;

public record GetAllGroupRequest(
    int? Quantity
) : IRequest<Result<GetAllGroupResponse>>;
