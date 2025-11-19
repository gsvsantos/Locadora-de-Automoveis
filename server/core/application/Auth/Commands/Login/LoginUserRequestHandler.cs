using FluentResults;
using LocadoraDeAutomoveis.Core.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Core.Application.Shared;
using LocadoraDeAutomoveis.Core.Domain.Auth;
using LocadoraDeAutomoveis.Core.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Core.Application.Auth.Commands.Login;

public class LoginUserRequestHandler(
    SignInManager<User> signInManager,
    UserManager<User> userManager,
    ITenantProvider tenantProvider,
    ITokenProvider tokenProvider,
    IUnitOfWork unitOfWork,
    ILogger<LoginUserRequestHandler> logger
) : IRequestHandler<LoginUserRequest, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(
        LoginUserRequest request, CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByNameAsync(request.UserName);

        if (user is null)
        {
            return Result.Fail(ErrorResults.NotFoundError("User not found."));
        }

        try
        {
            IList<string> userRoles = await userManager.GetRolesAsync(user);

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

            TokenResponse? accessToken = await tokenProvider.GenerateAccessToken(user) as TokenResponse;

            if (accessToken == null)
            {
                await unitOfWork.RollbackAsync();

                return Result.Fail(ErrorResults.InternalServerError(new Exception("Failed to generate access token")));
            }

            return Result.Ok(accessToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred during authentication. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
