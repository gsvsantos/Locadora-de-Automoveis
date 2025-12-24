using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Account.Commands.UpdateAccount;

public record UpdateAccountRequest(
    string FullName,
    string Email,
    string PhoneNumber,
    string Document,
    string State,
    string City,
    string Neighborhood,
    string Street,
    int Number
) : IRequest<Result<UpdateAccountResponse>>;
