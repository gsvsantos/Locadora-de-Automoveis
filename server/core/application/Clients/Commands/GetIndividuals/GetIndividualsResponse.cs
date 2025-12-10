using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.GetIndividuals;

public record GetIndividualsResponse(
    int Quantity,
    ImmutableList<DriverIndividualClientDto> Clients
);

public record DriverIndividualClientDto(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string? Document,
    string? LicenseNumber,
    DateTimeOffset? LicenseValidity
);
