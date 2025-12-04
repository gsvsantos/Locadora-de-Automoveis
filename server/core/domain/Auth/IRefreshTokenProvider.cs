using FluentResults;

namespace LocadoraDeAutomoveis.Domain.Auth;
public interface IRefreshTokenProvider
{
    Task<Result<RefreshToken>> GenerateRefreshTokenAsync(User user);
    Task<Result<(User User, Guid TenantId, RefreshToken NewRefreshToken)>> RotateRefreshTokenAsync(string refreshTokenString, CancellationToken ct);
    Task<Result> RevokeUserTokensAsync(Guid userId, string reason, CancellationToken ct);
}
