using FluentResults;
using LocadoraDeAutomoveis.Core.Application.Auth.DTOs;
using MediatR;

namespace LocadoraDeAutomoveis.Core.Application.Auth.Commands.Register;

public record RegisterUserRequest(
    string UserName,
    string FullName,
    string Email,
    string Password
) : IRequest<Result<TokenResponse>>;
