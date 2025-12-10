using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Partners.Commands.GetAll;

public record GetAllPartnerResponse(
    int Quantity,
    ImmutableList<GetAllPartnerDto> Partners
);

public record PartnerDto(
    Guid Id,
    string FullName,
    bool IsActive
);

public record GetAllPartnerDto(
    Guid Id,
    string FullName,
    bool IsActive,
    int CouponsQuantity
) : PartnerDto(
    Id,
    FullName,
    IsActive
);