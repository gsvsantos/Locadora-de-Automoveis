using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.ResetPassword;

public class ResetPasswordRequestHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    ILogger<ResetPasswordRequestHandler> logger
) : IRequestHandler<ResetPasswordRequest, Result>
{
    public async Task<Result> Handle(
        ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (!request.NewPassword.Equals(request.ConfirmNewPassword))
            {
                return Result.Fail(AuthErrorResults.NewPasswordConfirmationError());
            }

            User? user = await userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                return Result.Fail(ErrorResults.NotFoundError("User not found."));
            }

            IdentityResult result = await userManager.ResetPasswordAsync(
                user,
                request.Token,
                request.NewPassword
            );

            if (!result.Succeeded)
            {
                IEnumerable<string> erros = result.Errors
                    .Select(f => f.Description)
                    .ToList();
                return Result.Fail(ErrorResults.BadRequestError(erros));
            }

            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                await userManager.UpdateAsync(user);
            }

            await userManager.UpdateSecurityStampAsync(user);

            await unitOfWork.CommitAsync();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            logger.LogError(ex,
                "Error resetting password for {Email}",
                request.Email
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}