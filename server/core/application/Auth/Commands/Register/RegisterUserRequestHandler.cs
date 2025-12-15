using FluentResults;
using LocadoraDeAutomoveis.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Configurations;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.Register;

public class RegisterUserRequestHandler(
    UserManager<User> userManager,
    IRepositoryConfiguration repositoryConfiguration,
    ITokenProvider tokenProvider,
    IRefreshTokenProvider refreshTokenProvider,
    IRecaptchaService recaptchaService,
    IUnitOfWork unitOfWork,
    ILogger<RegisterUserRequestHandler> logger
) : IRequestHandler<RegisterUserRequest, Result<(AccessToken, IssuedRefreshTokenDto)>>
{
    public async Task<Result<(AccessToken, IssuedRefreshTokenDto)>> Handle(
        RegisterUserRequest request, CancellationToken cancellationToken)
    {
        if (!await recaptchaService.VerifyRecaptchaToken(request.RecaptchaToken))
        {
            return Result.Fail(ErrorResults.BadRequestError("Invalid reCAPTCHA verification"));
        }

        User user = new()
        {
            UserName = request.UserName,
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        try
        {
            if (!request.Password.Equals(request.ConfirmPassword))
            {
                return Result.Fail(AuthErrorResults.PasswordConfirmationError());
            }

            IdentityResult userResult = await userManager.CreateAsync(user, request.Password);

            if (!userResult.Succeeded)
            {
                IEnumerable<string> erros = userResult
                    .Errors
                    .Select(failure => failure.Description)
                    .ToList();

                await userManager.DeleteAsync(user);

                return Result.Fail(ErrorResults.BadRequestError(erros));
            }

            user.AssociateTenant(user.Id);

            await userManager.AddToRoleAsync(user, "Admin");

            await userManager.UpdateAsync(user);

            Configuration configutarion = new();
            configutarion.AssociateTenant(user.Id);
            configutarion.AssociateUser(user);
            await repositoryConfiguration.AddAsync(configutarion);

            AccessToken? accessToken = await tokenProvider.GenerateAccessToken(user) as AccessToken;

            if (accessToken is null)
            {
                return Result.Fail(ErrorResults.InternalServerError(new Exception("Failed to generate access token. Try again!")));
            }

            Result<IssuedRefreshTokenDto> refreshTokenResult = await refreshTokenProvider.GenerateRefreshTokenAsync(user);

            if (refreshTokenResult.IsFailed)
            {
                return Result.Fail(ErrorResults.InternalServerError(refreshTokenResult.Errors));
            }

            IssuedRefreshTokenDto? refreshToken = refreshTokenResult.Value;

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
