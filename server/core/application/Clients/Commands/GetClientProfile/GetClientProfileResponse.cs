namespace LocadoraDeAutomoveis.Application.Clients.Commands.GetClientProfile;

public record GetClientProfileResponse(
    ClientProfileDto Client
);

public record ClientProfileDto(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string? Document
);