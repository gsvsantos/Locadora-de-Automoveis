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
    EClientType ClientType,
    string Document
) : IRequest<Result<CreateClientResponse>>;
