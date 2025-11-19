using FluentResults;
using LocadoraDeAutomoveis.Core.Application.Auth.DTOs;
using MediatR;

namespace LocadoraDeAutomoveis.Core.Application.Auth.Commands.Login;

public record LoginUserRequest(
    string UserName,
    string Password
) : IRequest<Result<TokenResponse>>;
