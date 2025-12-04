using FluentResults;
using LocadoraDeAutomoveis.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.Register;

public class RegisterUserRequestHandler(
    UserManager<User> userManager,
    ITokenProvider tokenProvider,
    IRefreshTokenProvider refreshTokenProvider,
    IUnitOfWork unitOfWork,
    ILogger<RegisterUserRequestHandler> logger
) : IRequestHandler<RegisterUserRequest, Result<(AccessToken, RefreshToken)>>
{
    public async Task<Result<(AccessToken, RefreshToken)>> Handle(
        RegisterUserRequest request, CancellationToken cancellationToken)
    {
        User user = new()
        {
            UserName = request.UserName,
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        IdentityResult usuarioResult = await userManager.CreateAsync(user, request.Password);

        if (!usuarioResult.Succeeded)
        {
            IEnumerable<string> erros = usuarioResult
                .Errors
                .Select(failure => failure.Description)
                .ToList();

            await userManager.DeleteAsync(user);

            return Result.Fail(ErrorResults.BadRequestError(erros));
        }

        try
        {
            user.AssociateTenant(user.Id);

            await userManager.AddToRoleAsync(user, "Admin");

            await userManager.UpdateAsync(user);

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

            await userManager.DeleteAsync(user);

            logger.LogError(
                ex,
                "An error occurred during registration. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
