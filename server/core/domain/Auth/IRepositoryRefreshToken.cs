using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Domain.Auth;

public interface IRepositoryRefreshToken : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);
    Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<int> DeleteOldTokensAsync(DateTimeOffset dateLimit, CancellationToken ct = default);
}
