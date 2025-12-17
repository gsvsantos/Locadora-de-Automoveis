using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.ForgotPassword;

public record ForgotPasswordRequest(
    string Email
) : IRequest<Result>;
