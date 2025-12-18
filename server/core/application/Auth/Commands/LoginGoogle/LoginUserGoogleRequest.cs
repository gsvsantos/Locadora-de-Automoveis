using FluentResults;
using LocadoraDeAutomoveis.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Domain.Auth;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.LoginGoogle;

public record LoginUserGoogleRequest(
    string IdToken
) : IRequest<Result<(AccessToken, IssuedRefreshTokenDto)>>;
