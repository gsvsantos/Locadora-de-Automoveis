using FluentResults;
using LocadoraDeAutomoveis.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.Login;

public class LoginUserRequestHandler(
    SignInManager<User> signInManager,
    UserManager<User> userManager,
    ITokenProvider tokenProvider,
    IRefreshTokenProvider refreshTokenProvider,
    IUnitOfWork unitOfWork,
    ILogger<LoginUserRequestHandler> logger
) : IRequestHandler<LoginUserRequest, Result<(AccessToken, RefreshToken)>>
{
    public async Task<Result<(AccessToken, RefreshToken)>> Handle(
        LoginUserRequest request, CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByNameAsync(request.UserName);

        if (user is null)
        {
            return Result.Fail(ErrorResults.NotFoundError("User not found."));
        }

        try
        {
            SignInResult loginResult = await signInManager.PasswordSignInAsync(
                user.UserName!,
                request.Password,
                isPersistent: false,
                lockoutOnFailure: true
            );

            if (loginResult.IsLockedOut)
            {
                return Result.Fail(AuthErrorResults.UserLockedOutError());
            }

            if (loginResult.IsNotAllowed)
            {
                if (!await userManager.IsEmailConfirmedAsync(user) && user.Email != null)
                {
                    return Result.Fail(AuthErrorResults.PendingEmailConfirmationError(user.Email));
                }
                else if (!await userManager.IsPhoneNumberConfirmedAsync(user) && user.PhoneNumber != null)
                {
                    return Result.Fail(AuthErrorResults.PhoneNumberNotConfirmedError(user.PhoneNumber));
                }
                else
                {
                    return Result.Fail(AuthErrorResults.LoginNotAllowedError());
                }
            }

            if (loginResult.RequiresTwoFactor)
            {
                return Result.Fail(AuthErrorResults.TwoFactorRequiredError());
            }

            if (!loginResult.Succeeded)
            {
                return Result.Fail(AuthErrorResults.IncorrectCredentialsError());
            }

            AccessToken? accessToken = await tokenProvider.GenerateAccessToken(user) as AccessToken;

            if (accessToken is null)
            {
                return Result.Fail(ErrorResults.InternalServerError(new Exception("Failed to generate access token. Try again!")));
            }

            Result<RefreshToken> refreshTokenResult = await refreshTokenProvider.GenerateRefreshTokenAsync(user);

            if (refreshTokenResult.IsFailed)
            {
                return Result.Fail(ErrorResults.InternalServerError(refreshTokenResult.Errors));
            }

            RefreshToken? refreshToken = refreshTokenResult.Value;

            if (refreshToken is null)
            {
                return Result.Fail(ErrorResults.InternalServerError(new Exception("Failed to generate access token. Try again!")));
            }

            return Result.Ok((accessToken, refreshToken));
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            logger.LogError(
                ex,
                "An error occurred during login. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
