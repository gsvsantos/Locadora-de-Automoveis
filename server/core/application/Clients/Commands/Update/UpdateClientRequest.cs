using FluentResults;
using LocadoraDeAutomoveis.Domain.Clients;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.Update;

public record UpdateClientRequestPartial(
    string FullName,
    string Email,
    string PhoneNumber,
    string State,
    string City,
    string Neighborhood,
    string Street,
    int Number,
    EClientType Type,
    string Document
);

public record UpdateClientRequest(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string State,
    string City,
    string Neighborhood,
    string Street,
    int Number,
    EClientType Type,
    string Document
) : IRequest<Result<UpdateClientResponse>>;
