using FluentResults;
using MediatR;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.Logout;

public record LogoutUserRequest(
    string RefreshTokenPlain
) : IRequest<Result>;
