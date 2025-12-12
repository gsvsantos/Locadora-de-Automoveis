using FluentResults;
using Google.Apis.Auth;
using LocadoraDeAutomoveis.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Configurations;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.LoginGoogle;

public class LoginWithGoogleRequestHandler(
    UserManager<User> userManager,
    IRepositoryConfiguration repositoryConfiguration,
    ITokenProvider tokenProvider,
    IRefreshTokenProvider refreshTokenProvider,
    IConfiguration configuration
) : IRequestHandler<LoginWithGoogleRequest, Result<(AccessToken, RefreshToken)>>
{
    public async Task<Result<(AccessToken, RefreshToken)>> Handle(
        LoginWithGoogleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            GoogleJsonWebSignature.ValidationSettings settings = new()
            {
                Audience = [configuration["GOOGLE_CLIENT_ID"]]
            };

            GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

            User? user = await userManager.FindByEmailAsync(payload.Email);

            if (user is null)
            {
                user = new User
                {
                    UserName = payload.Email,
                    Email = payload.Email,
                    FullName = payload.Name,
                    EmailConfirmed = true,
                };

                IdentityResult userResult = await userManager.CreateAsync(user);

                user.AssociateTenant(user.Id);

                await userManager.AddToRoleAsync(user, "Admin");
                await userManager.UpdateAsync(user);

                Configuration configutarion = new();
                configutarion.AssociateTenant(user.Id);
                configutarion.AssociateUser(user);
                await repositoryConfiguration.AddAsync(configutarion);

                if (!userResult.Succeeded)
                {
                    return Result.Fail("Failed to create with Google Sign-In: " + userResult.Errors.First().Description);
                }
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
        catch (InvalidJwtException)
        {
            return Result.Fail("Token do Google inválido ou expirado.");
        }
        catch (Exception ex)
        {
            return Result.Fail("Erro interno no login Google: " + ex.Message);
        }
    }
}