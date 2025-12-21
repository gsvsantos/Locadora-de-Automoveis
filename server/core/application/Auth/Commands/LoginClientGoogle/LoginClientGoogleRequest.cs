using FluentResults;
using LocadoraDeAutomoveis.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Domain.Auth;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.LoginClientGoogle;

public record LoginClientGoogleRequest(
    string IdToken
) : IRequest<Result<(AccessToken, IssuedRefreshTokenDto)>>;
