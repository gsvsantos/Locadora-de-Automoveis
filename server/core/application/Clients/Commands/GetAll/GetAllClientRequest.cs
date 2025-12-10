using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.GetAll;

public record GetAllClientRequestPartial(
    int? Quantity,
    bool? IsActive
);

public record GetAllClientRequest(
    int? Quantity,
    bool? IsActive
) : IRequest<Result<GetAllClientResponse>>;
