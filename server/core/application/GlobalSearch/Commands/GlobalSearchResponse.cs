namespace LocadoraDeAutomoveis.Application.GlobalSearch.Commands;

public record GlobalSearchResponse(
    List<GlobalSearchItemDto> Items
);

public record GlobalSearchItemDto(
    Guid Id,
    string Title,
    string Description,
    string Type,
    string Route
);