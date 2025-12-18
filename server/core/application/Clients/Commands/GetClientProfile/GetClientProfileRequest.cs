using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Clients.Commands.GetClientProfile;

public record GetClientProfileRequest() : IRequest<Result<GetClientProfileResponse>>;
