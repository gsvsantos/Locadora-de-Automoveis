using FluentResults;
using LocadoraDeAutomoveis.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.Refresh;

public class RefreshTokenRequestHandler(
    UserManager<User> userManager,
    ITokenProvider tokenProvider,
    IRefreshTokenProvider refreshTokenProvider,
    IUnitOfWork unitOfWork,
    ILogger<RefreshTokenRequestHandler> logger
) : IRequestHandler<RefreshTokenRequest, Result<(AccessToken, IssuedRefreshTokenDto)>>
{
    public async Task<Result<(AccessToken, IssuedRefreshTokenDto)>> Handle(
        RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Result<(User User, Guid TenantId, IssuedRefreshTokenDto NewRefreshToken)> result =
                            await refreshTokenProvider.RotateRefreshTokenAsync(request.RefreshTokenString, cancellationToken);

            if (result.IsFailed)
            {
                return Result.Fail(ErrorResults.InternalServerError(result.Errors));
            }

            (User user, _, IssuedRefreshTokenDto newRefreshToken) = result.Value;

            user.AccessTokenVersionId = Guid.NewGuid();
            await userManager.UpdateAsync(user);

            AccessToken? newAccessToken = await tokenProvider.GenerateAccessToken(user) as AccessToken;
            if (newAccessToken is null)
            {
                return Result.Fail(ErrorResults.InternalServerError(new Exception("Failed to generate access token.")));
            }

            await unitOfWork.CommitAsync();

            return Result.Ok((newAccessToken, newRefreshToken));

        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            logger.LogError(
                ex,
                "An error occurred during refresh. \n{@Request}.", request
            );

            return Result.Fail(ErrorResults.InternalServerError(ex));
        }
    }
}
