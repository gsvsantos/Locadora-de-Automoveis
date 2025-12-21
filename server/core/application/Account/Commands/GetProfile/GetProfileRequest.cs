using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Account.Commands.GetProfile;

public record GetProfileRequest() : IRequest<Result<GetProfileResponse>>;
