using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Configurations.Commands.Details;

public record DetailsRequest() : IRequest<Result<DetailsResponse>>;
