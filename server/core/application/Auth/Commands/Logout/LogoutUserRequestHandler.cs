using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.Logout;

public class LogoutUserRequestHandler(
    UserManager<User> userManager,
    IRepositoryRefreshToken repositoryRefreshToken,
    IRefreshTokenProvider refreshTokenProvider,
    RefreshTokenOptions refreshTokenOptions,
    IUnitOfWork unitOfWork,
    ILogger<LogoutUserRequestHandler> logger
) : IRequestHandler<LogoutUserRequest, Result>
{
    public async Task<Result> Handle(
        LogoutUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            string refreshTokenHash = Hash(request.RefreshTokenPlain);

            RefreshToken? refreshToken = await repositoryRefreshToken.GetByTokenHashAsync(refreshTokenHash, cancellationToken);

            if (refreshToken is null)
            {
                return Result.Fail(ErrorResults.InternalServerError(new Exception("Something went wrong! Refresh token not found.")));
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

    private string Hash(string plainTextValue)
    {
        using HMACSHA256 hmac = new(Encoding.UTF8.GetBytes(refreshTokenOptions.PepperSecret));
        return Convert.ToHexString(
            hmac.ComputeHash(Encoding.UTF8.GetBytes(plainTextValue))
        );
    }
}
