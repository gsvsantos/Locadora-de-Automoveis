using LocadoraDeAutomoveis.Application.RentalExtras.Commands.GetAll;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.RentalExtras.Commands.GetAllDistinct;

public record GetAllDistinctExtraResponse(
    int Quantity,
    ImmutableList<RentalExtraDto> Extras
);