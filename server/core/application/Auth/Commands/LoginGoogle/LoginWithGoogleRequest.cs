using FluentResults;
using LocadoraDeAutomoveis.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Domain.Auth;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.LoginGoogle;

public record LoginWithGoogleRequest(
    string IdToken
) : IRequest<Result<(AccessToken AccessToken, IssuedRefreshTokenDto RefreshToken)>>;
