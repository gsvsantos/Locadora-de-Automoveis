using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using LocadoraDeAutomoveis.Application.Auth.Commands.Register;
using LocadoraDeAutomoveis.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Clients;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.RegisterClient;

public class RegisterClientRequestHandler(
    UserManager<User> userManager,
    IMapper mapper,
    IRepositoryClient repositoryClient,
    ITokenProvider tokenProvider,
    IRefreshTokenProvider refreshTokenProvider,
    IRecaptchaService recaptchaService,
    IUnitOfWork unitOfWork,
    IAuthEmailService emailService,
    IValidator<Client> validator,
    ILogger<RegisterUserRequestHandler> logger
) : IRequestHandler<RegisterClientRequest, Result<(AccessToken, IssuedRefreshTokenDto)>>
{
    public async Task<Result<(AccessToken, IssuedRefreshTokenDto)>> Handle(
        RegisterClientRequest request, CancellationToken cancellationToken)
    {
        if (!await recaptchaService.VerifyRecaptchaToken(request.RecaptchaToken))
        {
            return Result.Fail(ErrorResults.BadRequestError("Invalid reCAPTCHA verification"));
        }

        User userLogin = mapper.Map<User>(request);

        try
        {
            if (!request.Password.Equals(request.ConfirmPassword))
            {
                return Result.Fail(AuthErrorResults.PasswordConfirmationError());
            }

            IdentityResult userResult = await userManager.CreateAsync(userLogin, request.Password);

            if (!userResult.Succeeded)
            {
                IEnumerable<string> erros = userResult
                    .Errors
                    .Select(failure => failure.Description)
                    .ToList();

                return Result.Fail(ErrorResults.BadRequestError(erros));
            }

            await userManager.AddToRoleAsync(userLogin, "Client");

            userLogin.AssociateTenant(Guid.Empty);

            Client client = mapper.Map<Client>(request);

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

            try
            {
                await emailService.ScheduleClientRegisterWelcome(client.Email, client.FullName, client.PreferredLanguage);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to schedule welcome email for client {ClientId}", client.Id);
            }

            return Result.Ok((accessToken, refreshToken));
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            if (userLogin.Id != Guid.Empty)
            {
                await userManager.DeleteAsync(userLogin);
            }

            logger.LogError(
                ex,
                "An error occurred during registration. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }

    private static bool DocumentAlreadyRegistred(Client client, List<Client> existingClients)
    {
        return existingClients
            .Any(entity => string.Equals(
                entity.Document,
                client.Document,
                StringComparison.CurrentCultureIgnoreCase)
            );
    }
}