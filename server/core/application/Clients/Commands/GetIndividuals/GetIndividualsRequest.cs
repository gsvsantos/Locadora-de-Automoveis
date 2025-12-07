using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Clients.GetIndividuals;

public record GetIndividualsRequest(
    Guid Id
) : IRequest<Result<GetIndividualsResponse>>;
