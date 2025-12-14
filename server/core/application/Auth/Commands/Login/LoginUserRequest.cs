using FluentResults;
using LocadoraDeAutomoveis.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Domain.Auth;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.Login;

public record LoginUserRequest(
    string UserName,
    string Password,
    string RecaptchaToken
) : IRequest<Result<(AccessToken AccessToken, IssuedRefreshTokenDto RefreshToken)>>;
