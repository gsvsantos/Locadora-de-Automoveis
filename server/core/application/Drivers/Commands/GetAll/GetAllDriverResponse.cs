using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Drivers.Commands.GetAll;

public record GetAllDriverResponse(
    int Quantity,
    ImmutableList<DriverDto> Drivers
);

public record DriverDto(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string Document,
    string LicenseNumber,
    DateTimeOffset LicenseValidity,
    string ClientName
);
