using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.ForgotPassword;

public class ForgotPasswordRequestHandler(
    UserManager<User> userManager,
    IAuthEmailService emailService,
    ILogger<ForgotPasswordRequestHandler> logger
) : IRequestHandler<ForgotPasswordRequest, Result>
{
    public async Task<Result> Handle(
        ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Result.Fail(ErrorResults.NotFoundError("User not found."));
        }

        try
        {
            string token = await userManager.GeneratePasswordResetTokenAsync(user);

            try
            {
                await emailService.SendForgotPasswordEmailAsync(user.Email!, user.FullName, token, request.IsPortal, user.PreferredLanguage);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to send forgot password email for user {UserId}", user.Id
                );

                return Result.Fail(ErrorResults.InternalServerError(new Exception("Failed to send recovery email.")));
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error processing ForgotPassword for {Email}", request.Email
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
