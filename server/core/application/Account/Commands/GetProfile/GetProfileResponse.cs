namespace LocadoraDeAutomoveis.Application.Account.Commands.GetClientProfile;

public record GetProfileResponse(
    ClientProfileDto Client
);

public record ClientProfileDto(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string? Document
);