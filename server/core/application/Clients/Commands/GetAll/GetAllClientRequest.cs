using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.GetAll;

public record GetAllClientRequest(
    int? Quantity
) : IRequest<Result<GetAllClientResponse>>;
