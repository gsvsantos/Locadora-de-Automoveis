using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Groups.Commands.GetAllDistinct;

public record GetAllDistinctGroupRequest(
) : IRequest<Result<GetAllDistinctGroupResponse>>;
