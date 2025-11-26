using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.RateServices.Commands.GetAll;

public record GetAllRateServiceResponse(
    int Quantity,
    ImmutableList<RateServiceDto> RateServices
);

public record RateServiceDto(
    Guid Id,
    string Name,
    decimal Price,
    bool IsFixed,
    string RateType
);