using FluentResults;
using LocadoraDeAutomoveis.Application.Groups.Commands.GetAll;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Groups.Commands.GetAllDistinct;

public record GetAllDistinctGroupsRequest : IRequest<Result<List<GroupDto>>>;
