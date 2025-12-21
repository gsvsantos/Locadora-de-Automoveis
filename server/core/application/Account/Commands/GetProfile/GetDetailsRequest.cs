using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Account.Commands.GetProfile;

public record GetDetailsRequest() : IRequest<Result<GetDetailsResponse>>;
