using LocadoraDeAutomoveis.Application.Drivers.Commands.GetAll;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Drivers.Commands.GetAllDistinct;

public record GetAllDistinctDriverResponse(
    int Quantity,
    ImmutableList<DriverDto> Drivers
);
