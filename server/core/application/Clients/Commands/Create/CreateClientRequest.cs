using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.Create;
public record CreateClientRequest(
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
) : IRequest<Result<CreateClientResponse>>;
