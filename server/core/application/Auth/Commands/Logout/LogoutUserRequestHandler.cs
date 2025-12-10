using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.Logout;

public class LogoutUserRequestHandler(
    UserManager<User> userManager,
    IRepositoryRefreshToken repositoryRefreshToken,
    IRefreshTokenProvider refreshTokenProvider,
    IUnitOfWork unitOfWork,
    ILogger<LogoutUserRequestHandler> logger
) : IRequestHandler<LogoutUserRequest, Result>
{
    public async Task<Result> Handle(
        LogoutUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            RefreshToken? refreshToken = await repositoryRefreshToken.GetByTokenHashAsync(request.RefreshTokenHash);

            if (refreshToken is null)
            {
                return Result.Fail(ErrorResults.InternalServerError(new Exception("Failed to generate access token. Try again!")));
            }

            Guid userId = refreshToken.UserAuthenticatedId;

            await refreshTokenProvider.RevokeUserTokensAsync(userId, "Logout", cancellationToken);

            User? user = await userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                return Result.Fail(ErrorResults.NotFoundError("User not found."));
            }

            user.AccessTokenVersionId = Guid.NewGuid();

            await unitOfWork.CommitAsync();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            logger.LogError(
                ex,
                "An error occurred during logout. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
