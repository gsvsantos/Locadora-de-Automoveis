using LocadoraDeAutomoveis.Domain.Clients;

namespace LocadoraDeAutomoveis.Application.Account.Commands.GetProfile;

public record GetProfileResponse(
    ClientProfileDto Client
);

public record ClientProfileDto(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string? Document,
    Address? Address
);