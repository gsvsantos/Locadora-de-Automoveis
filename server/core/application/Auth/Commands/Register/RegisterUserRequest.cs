using FluentResults;
using LocadoraDeAutomoveis.Application.Auth.DTOs;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.Register;

public record RegisterUserRequest(
    string UserName,
    string FullName,
    string Email,
    string PhoneNumber,
    string Password
) : IRequest<Result<TokenResponse>>;
