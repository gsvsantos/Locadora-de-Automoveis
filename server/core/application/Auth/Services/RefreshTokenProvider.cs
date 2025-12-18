using FluentResults;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace LocadoraDeAutomoveis.Application.Auth.Services;

public class RefreshTokenProvider(
    IRepositoryRefreshToken repositoryRefreshToken,
    IUnitOfWork unitOfWork,
    UserManager<User> userManager,
    IHttpContextAccessor contextAccessor,
    RefreshTokenOptions refreshTokenOptions
) : IRefreshTokenProvider
{
    private readonly HttpContext? httpContext = contextAccessor.HttpContext;
    private readonly TimeSpan expirationInDays = TimeSpan.FromDays(refreshTokenOptions.ExpirationInDays);
    private readonly string pepperSecret = refreshTokenOptions.PepperSecret;

    public async Task<Result<IssuedRefreshTokenDto>> GenerateRefreshTokenAsync(User user)
    {
        if (user.Id.Equals(Guid.Empty) || user.TenantId.Equals(Guid.Empty))
        {
            return Result.Fail("Invalid parameters.");
        }

        string plainToken = GenerateOpaqueToken();

        string tokenHash = Hash(plainToken);

        DateTimeOffset now = DateTimeOffset.UtcNow;

        RefreshToken token = new()
        {
            UserAuthenticatedId = user.Id,
            TokenHash = tokenHash,
            CreatedDateUtc = now,
            ExpirationDateUtc = now.Add(this.expirationInDays),
            CreationIp = this.httpContext?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty,
            UserAgent = this.httpContext?.Request.Headers[HeaderNames.UserAgent].ToString() ?? string.Empty,
        };
        token.AssociateTenant(user.GetTenantId());
        token.AssociateUser(user);

        await repositoryRefreshToken.AddAsync(token);

        await unitOfWork.CommitAsync();

        return Result.Ok(new IssuedRefreshTokenDto(
            plainToken,
            token.ExpirationDateUtc
        ));
    }

    public async Task<Result<(User User, Guid TenantId, IssuedRefreshTokenDto NewRefreshToken)>> RotateRefreshTokenAsync(string refreshTokenString, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(refreshTokenString))
        {
            return Result.Fail("Missing refresh token.");
        }

        string tokenHash = Hash(refreshTokenString);

        RefreshToken? token = await repositoryRefreshToken.GetByTokenHashAsync(tokenHash, ct);

        if (token is null)
        {
            return Result.Fail("Invalid refresh token.");
        }

        if (!token.IsValid)
        {
            return Result.Fail("Refresh token expired or revoked.");
        }

        User? user = await userManager.FindByIdAsync(token.UserId.ToString());

        if (user is null)
        {
            return Result.Fail(ErrorResults.NotFoundError("User not found."));
        }

        string newRefreshTokenPlain = GenerateOpaqueToken();

        string newRefreshTokenHash = Hash(newRefreshTokenPlain);

        DateTimeOffset now = DateTimeOffset.UtcNow;

        token.RevokedDateUtc = now;
        token.ReplacedByTokenHash = newRefreshTokenHash;
        token.RevocationReason = "Rotation";

        await repositoryRefreshToken.UpdateAsync(token.Id, token);

        RefreshToken newToken = new()
        {
            UserAuthenticatedId = token.UserAuthenticatedId,
            TokenHash = newRefreshTokenHash,
            CreatedDateUtc = now,
            ExpirationDateUtc = now.Add(this.expirationInDays),
            CreationIp = this.httpContext?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty,
            UserAgent = this.httpContext?.Request.Headers[HeaderNames.UserAgent].ToString() ?? string.Empty,
        };
        newToken.AssociateTenant(token.GetTenantId());
        newToken.AssociateUser(user);

        await repositoryRefreshToken.AddAsync(newToken);

        await unitOfWork.CommitAsync();

        return Result.Ok((user, token.GetTenantId(), new IssuedRefreshTokenDto(
                newRefreshTokenPlain,
                newToken.ExpirationDateUtc
        )));
    }

    public async Task<Result> RevokeUserTokensAsync(Guid userId, string reason, CancellationToken ct)
    {
        List<RefreshToken> tokens = await repositoryRefreshToken.GetActiveByUserIdAsync(userId, ct);

        if (tokens.Count == 0)
        {
            return Result.Ok();
        }

        DateTimeOffset now = DateTimeOffset.UtcNow;
        foreach (RefreshToken token in tokens)
        {
            token.RevokedDateUtc = now;
            token.RevocationReason = reason;
            await repositoryRefreshToken.UpdateAsync(token.Id, token);
        }

        await unitOfWork.CommitAsync();

        return Result.Ok();
    }

    private string Hash(string plainTextValue)
    {
        using HMACSHA256 hmac = new(Encoding.UTF8.GetBytes(this.pepperSecret));
        return Convert.ToHexString(
            hmac.ComputeHash(Encoding.UTF8.GetBytes(plainTextValue))
        );
    }

    private static string GenerateOpaqueToken()
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
}
