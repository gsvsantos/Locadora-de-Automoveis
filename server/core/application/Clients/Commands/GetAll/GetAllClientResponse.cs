using LocadoraDeAutomoveis.Domain.Clients;
using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.GetAll;

public record GetAllClientResponse(
    int Quantity,
    ImmutableList<ClientDto> Clients
);

public record ClientDto(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    EClientType ClientType,
    string State,
    string City,
    string Neighborhood,
    string Street,
    int Number,
    string? Document,
    string? LicenseNumber
);

