using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.ResetPassword;

public record ResetPasswordRequest(
    string Email,
    string Token,
    string NewPassword,
    string ConfirmNewPassword
) : IRequest<Result>;