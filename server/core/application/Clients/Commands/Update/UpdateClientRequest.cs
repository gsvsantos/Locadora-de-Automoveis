using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.Update;

public record UpdateClientRequestPartial(
    string FullName,
    string Email,
    string PhoneNumber,
    bool IsJurifical,
    string State,
    string City,
    string Neighborhood,
    string Street,
    int Number,
    string Document
);

public record UpdateClientRequest(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    bool IsJurifical,
    string State,
    string City,
    string Neighborhood,
    string Street,
    int Number,
    string Document
) : IRequest<Result<UpdateClientResponse>>;
