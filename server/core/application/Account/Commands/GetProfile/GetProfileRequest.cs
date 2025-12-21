using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Account.Commands.GetClientProfile;

public record GetProfileRequest() : IRequest<Result<GetProfileResponse>>;
