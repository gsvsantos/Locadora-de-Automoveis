using FluentResults;
using LocadoraDeAutomoveis.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Domain.Auth;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.Refresh;

public record RefreshTokenRequest(
    string RefreshTokenString
) : IRequest<Result<(AccessToken AccessToken, IssuedRefreshTokenDto RefreshToken)>>;
