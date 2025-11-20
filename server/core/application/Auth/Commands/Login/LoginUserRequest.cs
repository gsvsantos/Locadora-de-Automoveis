using FluentResults;
using LocadoraDeAutomoveis.Application.Auth.DTOs;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.Login;

public record LoginUserRequest(
    string UserName,
    string Password
) : IRequest<Result<TokenResponse>>;
