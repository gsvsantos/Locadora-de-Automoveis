using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Google.Apis.Auth;
using LocadoraDeAutomoveis.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.LoginClientGoogle;

public class LoginClientGoogleRequestHandler(
    UserManager<User> userManager,
    IMapper mapper,
    IRepositoryClient repositoryClient,
    ITokenProvider tokenProvider,
    IRefreshTokenProvider refreshTokenProvider,
    IConfiguration configuration,
    IUnitOfWork unitOfWork,
    IAuthEmailService emailService,
    IValidator<Client> validator,
    ILogger<LoginClientGoogleRequestHandler> logger
) : IRequestHandler<LoginClientGoogleRequest, Result<(AccessToken, IssuedRefreshTokenDto)>>
{
    public async Task<Result<(AccessToken, IssuedRefreshTokenDto)>> Handle(
        LoginClientGoogleRequest request, CancellationToken cancellationToken)
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
                userLogin = mapper.Map<User>(payload);

                IdentityResult userResult = await userManager.CreateAsync(userLogin);

                if (!userResult.Succeeded)
                {
                    IEnumerable<string> errors = userResult
                        .Errors
                        .Select(failure => failure.Description)
                        .ToList();

                    return Result.Fail(ErrorResults.BadRequestError(errors));
                }

                await userManager.AddToRoleAsync(userLogin, "Client");

                userLogin.AssociateTenant(Guid.Empty);
                await userManager.UpdateAsync(userLogin);

                Client client = mapper.Map<Client>(payload);

                client.DefineType(EClientType.Individual);

                ValidationResult validationResult = await validator.ValidateAsync(client, cancellationToken);

                if (!validationResult.IsValid)
                {
                    await userManager.DeleteAsync(userLogin);

                    List<string> errors = validationResult.Errors
                        .Select(failure => failure.ErrorMessage)
                        .ToList();

                    return Result.Fail(ErrorResults.BadRequestError(errors));
                }

                client.AssociateLoginUser(userLogin);
                client.AssociateUser(userLogin);
                client.AssociateTenant(Guid.Empty);

                await repositoryClient.AddAsync(client);
                await unitOfWork.CommitAsync();

                if (!userResult.Succeeded)
                {
                    await userManager.DeleteAsync(userLogin);

                    return Result.Fail("Failed to create with Google Sign-In: " + userResult.Errors.First().Description);
                }

                string token = await userManager.GeneratePasswordResetTokenAsync(userLogin);

                try
                {
                    await emailService.ScheduleClientGoogleWelcome(client.Email, client.FullName, token, client.PreferredLanguage);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to schedule google welcome email for client {ClientiD}", client.Id);
                }
            }

            IList<string> userRoles = await userManager.GetRolesAsync(userLogin!);

            if (!userRoles.Contains("Client"))
            {
                return Result.Fail(AuthErrorResults.UserNotClientError());
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
