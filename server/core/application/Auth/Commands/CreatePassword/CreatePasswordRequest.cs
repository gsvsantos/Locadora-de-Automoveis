using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.CreatePassword;

public record CreatePasswordRequestPartial(
    string NewPassword,
    string ConfirmNewPassword
);

public record CreatePasswordRequest(
    string RefreshTokenPlain,
    string NewPassword,
    string ConfirmNewPassword
) : IRequest<Result>;