using LocadoraDeAutomoveis.Domain.Clients;

namespace LocadoraDeAutomoveis.Application.Account.Commands.GetAccountDetails;

public record GetAccountDetailsResponse(
    ClientProfileDto Client
);

public record ClientProfileDto(
    string FullName,
    string Email,
    string PhoneNumber,
    string? Document,
    Address? Address
);