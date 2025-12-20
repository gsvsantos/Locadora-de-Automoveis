using LocadoraDeAutomoveis.Application.Groups.Commands.GetAll;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Groups.Commands.GetAllDistinct;

public record GetAllDistinctGroupResponse(
    int Quantity,
    ImmutableList<GroupDto> Groups
);