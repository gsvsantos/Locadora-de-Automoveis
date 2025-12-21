using FluentResults;
using LocadoraDeAutomoveis.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Domain.Auth;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.RegisterClient;

public record RegisterClientRequest(
    string UserName,
    string FullName,
    string Email,
    string PhoneNumber,
    string Password,
    string ConfirmPassword,
    string RecaptchaToken
) : IRequest<Result<(AccessToken, IssuedRefreshTokenDto)>>;
