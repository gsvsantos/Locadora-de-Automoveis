using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.ChangePassword;

public record ChangePasswordRequestPartial(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword
);

public record ChangePasswordRequest(
    string RefreshTokenPlain,
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword
) : IRequest<Result>;
