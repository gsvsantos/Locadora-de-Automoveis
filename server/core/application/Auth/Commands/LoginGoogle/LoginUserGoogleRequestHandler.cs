using FluentResults;
using Google.Apis.Auth;
using LocadoraDeAutomoveis.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Configurations;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.LoginGoogle;

public class LoginUserGoogleRequestHandler(
    UserManager<User> userManager,
    IRepositoryConfiguration repositoryConfiguration,
    ITokenProvider tokenProvider,
    IRefreshTokenProvider refreshTokenProvider,
    IConfiguration configuration,
    IAuthEmailService emailService,
    ILogger<LoginUserGoogleRequestHandler> logger
) : IRequestHandler<LoginUserGoogleRequest, Result<(AccessToken, IssuedRefreshTokenDto)>>
{
    public async Task<Result<(AccessToken, IssuedRefreshTokenDto)>> Handle(
        LoginUserGoogleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            GoogleJsonWebSignature.ValidationSettings settings = new()
            {
                Audience = [configuration["GOOGLE_CLIENT_ID"]]
            };

            GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

            User? userLogin = await userManager.FindByEmailAsync(payload.Email);

            if (userLogin is null)
            {
                userLogin = new User
                {
                    UserName = payload.Email,
                    Email = payload.Email,
                    FullName = payload.Name,
                    EmailConfirmed = true,
                };

                IdentityResult userResult = await userManager.CreateAsync(userLogin);

                if (!userResult.Succeeded)
                {
                    IEnumerable<string> errors = userResult
                        .Errors
                        .Select(failure => failure.Description)
                        .ToList();

                    return Result.Fail(ErrorResults.BadRequestError(errors));
                }

                await userManager.AddToRoleAsync(userLogin, "Admin");

                userLogin.AssociateTenant(userLogin.Id);

                await userManager.UpdateAsync(userLogin);

                Configuration configutarion = new();
                configutarion.AssociateTenant(userLogin.Id);
                configutarion.AssociateUser(userLogin);
                await repositoryConfiguration.AddAsync(configutarion);

                if (!userResult.Succeeded)
                {
                    await userManager.DeleteAsync(userLogin);

                    return Result.Fail("Failed to create with Google Sign-In: " + userResult.Errors.First().Description);
                }

                string token = await userManager.GeneratePasswordResetTokenAsync(userLogin);

                try
                {
                    await emailService.ScheduleBusinessGoogleWelcome(userLogin.Email, userLogin.FullName, token, userLogin.PreferredLanguage);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to schedule google welcome email for userLogin {UseriD}", userLogin.Id);
                }
            }

            IList<string> userRoles = await userManager.GetRolesAsync(userLogin!);

            if (userRoles.Contains("Client"))
            {
                return Result.Fail(AuthErrorResults.UserIsClientError());
            }

            AccessToken? accessToken = await tokenProvider.GenerateAccessToken(userLogin) as AccessToken;

            if (accessToken is null)
            {
                return Result.Fail(ErrorResults.InternalServerError(new Exception("Failed to generate access token. Try again!")));
            }

            Result<IssuedRefreshTokenDto> refreshTokenResult = await refreshTokenProvider.GenerateRefreshTokenAsync(userLogin);

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
        catch (InvalidJwtException)
        {
            return Result.Fail("Invalid Google Token.");
        }
        catch (Exception ex)
        {
            return Result.Fail("Something  Google: " + ex.Message);
        }
    }
}
