using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.ChangePassword;
public class ChangePasswordRequestHandler(
    UserManager<User> userManager,
    IRepositoryRefreshToken repositoryRefreshToken,
    IRefreshTokenProvider refreshTokenProvider,
    RefreshTokenOptions refreshTokenOptions,
    IUnitOfWork unitOfWork,
    ILogger<ChangePasswordRequestHandler> logger
) : IRequestHandler<ChangePasswordRequest, Result>
{
    public async Task<Result> Handle(
        ChangePasswordRequest request, CancellationToken cancellationToken)
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

            await refreshTokenProvider.RevokeUserTokensAsync(userId, "ChangePassword", cancellationToken);

            User? user = await userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                return Result.Fail(ErrorResults.NotFoundError("User not found."));
            }

            bool userPasswordMatch = await userManager.CheckPasswordAsync(user, request.CurrentPassword);

            if (!userPasswordMatch)
            {
                return Result.Fail(AuthErrorResults.IncorrectCurrentPasswordError());
            }

            if (!request.NewPassword.Equals(request.ConfirmNewPassword))
            {
                return Result.Fail(AuthErrorResults.NewPasswordConfirmationError());
            }

            IdentityResult result = await userManager.ChangePasswordAsync(
                user,
                request.CurrentPassword,
                request.NewPassword
            );

            if (!result.Succeeded)
            {
                IEnumerable<string> erros = result
                    .Errors
                    .Select(failure => failure.Description)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(erros));
            }

            user.AccessTokenVersionId = Guid.NewGuid();

            await userManager.UpdateAsync(user);

            await unitOfWork.CommitAsync();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            logger.LogError(
                ex,
                "An error occurred during the request. \n{@Request}.", request
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
