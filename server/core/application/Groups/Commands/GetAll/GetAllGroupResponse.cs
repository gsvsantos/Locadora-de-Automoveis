using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Groups.Commands.GetAll;

public record GetAllGroupResponse(
    int Quantity,
    ImmutableList<GroupDto> Groups
);

public record GroupDto(
    Guid Id,
    string Name,
    bool IsActive
);
