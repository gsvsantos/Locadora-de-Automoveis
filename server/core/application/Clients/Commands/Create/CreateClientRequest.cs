using FluentResults;
using LocadoraDeAutomoveis.Domain.Clients;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.Create;
public record CreateClientRequest(
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
) : IRequest<Result<CreateClientResponse>>;
