using FluentResults;

namespace LocadoraDeAutomoveis.Domain.Auth;
public interface IRefreshTokenProvider
{
    Task<Result<IssuedRefreshTokenDto>> GenerateRefreshTokenAsync(User user);
    Task<Result<(User User, Guid TenantId, IssuedRefreshTokenDto NewRefreshToken)>> RotateRefreshTokenAsync(string refreshTokenString, CancellationToken ct);
    Task<Result> RevokeUserTokensAsync(Guid userId, string reason, CancellationToken ct);
}
