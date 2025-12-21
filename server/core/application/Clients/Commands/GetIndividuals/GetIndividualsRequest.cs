using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.GetIndividuals;

public record GetIndividualsRequest(
    Guid Id
) : IRequest<Result<GetIndividualsResponse>>;
