using FluentResults;
using LocadoraDeAutomoveis.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocadoraDeAutomoveis.Application.Auth.Commands.Refresh;

public class RefreshTokenRequestHandler(
    ITokenProvider tokenProvider,
    IRefreshTokenProvider refreshTokenProvider,
    IUnitOfWork unitOfWork,
    ILogger<RefreshTokenRequestHandler> logger
) : IRequestHandler<RefreshTokenRequest, Result<(AccessToken, RefreshToken)>>
{
    public async Task<Result<(AccessToken, RefreshToken)>> Handle(
        RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Result<(User User, Guid TenantId, RefreshToken NewRefreshToken)> result = await refreshTokenProvider.RotateRefreshTokenAsync(request.RefreshTokenString, cancellationToken);

            if (result.IsFailed)
            {
                return Result.Fail(ErrorResults.InternalServerError(result.Errors));
            }

            (User user, Guid tenantId, RefreshToken newRefreshToken) = result.Value;

            user.AccessTokenVersionId = Guid.NewGuid();

            AccessToken? newAccessToken = await tokenProvider.GenerateAccessToken(user) as AccessToken;

            if (newAccessToken is null)
            {
                return Result.Fail(ErrorResults.InternalServerError(new Exception("Failed to generate the newaccess token. Try again!")));
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
