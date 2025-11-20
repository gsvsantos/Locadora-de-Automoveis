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
    IUnitOfWork unitOfWork,
    ILogger<RegisterUserRequestHandler> logger
) : IRequestHandler<RegisterUserRequest, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(
        RegisterUserRequest request, CancellationToken cancellationToken)
    {
        User user = new()
        {
            UserName = request.UserName,
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        try
        {
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

            await userManager.AddToRoleAsync(user, "Admin");

            TokenResponse? accessToken = await tokenProvider.GenerateAccessToken(user) as TokenResponse;

            if (accessToken == null)
            {
                await unitOfWork.RollbackAsync();

                return Result.Fail(ErrorResults.InternalServerError(new Exception("Failed to generate access token.")));
            }

            return Result.Ok(accessToken);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            logger.LogError(
                ex,
                "An error occurred during registration. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
