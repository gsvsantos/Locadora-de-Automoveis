using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Drivers.Commands.Create;

public record CreateDriverRequest(
    Guid ClientId,
    string FullName,
    string Email,
    string PhoneNumber,
    string Document,
    string LicenseNumber,
    DateTimeOffset LicenseValidity
) : IRequest<Result<CreateDriverResponse>>;
