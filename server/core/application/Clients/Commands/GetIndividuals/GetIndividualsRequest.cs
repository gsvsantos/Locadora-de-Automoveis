using FluentResults;
using LocadoraDeAutomoveis.Application.Clients.Commands.GetIndividuals;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Clients.GetIndividuals;

public record GetIndividualsRequest(
    Guid Id
) : IRequest<Result<GetIndividualsResponse>>;
