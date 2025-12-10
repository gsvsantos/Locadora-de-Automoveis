using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace LocadoraDeAutomoveis.Infrastructure.Auth;

public class RepositoryRefreshToken(AppDbContext context)
    : BaseRepository<RefreshToken>(context), IRepositoryRefreshToken
{
    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default)
    {
        return await this.records
            .FirstOrDefaultAsync(t => t.TokenHash.Equals(tokenHash), ct);
    }

    public async Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await this.records
            .Where(t =>
                t.UserId.Equals(userId) &&
                t.RevokedDateUtc == null &&
                t.ExpirationDateUtc >= DateTimeOffset.UtcNow)
            .ToListAsync(ct);
    }

    public async Task<int> DeleteOldTokensAsync(DateTimeOffset dateLimit, CancellationToken ct = default)
    {

        int quantidadeRemovida = await this.records
            .Where(token =>
                (token.RevokedDateUtc.HasValue && token.RevokedDateUtc.Value < dateLimit) ||
                (!token.RevokedDateUtc.HasValue && token.ExpirationDateUtc < dateLimit))
            .ExecuteDeleteAsync(ct);

        return quantidadeRemovida;
    }
}
