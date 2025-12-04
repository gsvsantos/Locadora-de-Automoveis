using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.RentalExtras.Commands.GetAll;

public record GetAllRentalExtraResponse(
    int Quantity,
    ImmutableList<RentalExtraDto> RentalExtras
);

public record RentalExtraDto(
    Guid Id,
    string Name,
    decimal Price,
    bool IsChargedPerDay,
    string Type
);