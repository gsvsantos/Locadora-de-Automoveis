using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.WebApi.Jobs;

public class RefreshTokenCleanupJob(
    IRepositoryRefreshToken repositoryRefreshToken,
    IUnitOfWork unitOfWork,
    ILogger<RefreshTokenCleanupJob> logger
)
{
    private static readonly TimeSpan RetentionPeriod = TimeSpan.FromDays(90);

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        DateTimeOffset dateLimit = DateTimeOffset.UtcNow.Subtract(RetentionPeriod);

        int quantityRemoved = await repositoryRefreshToken
            .DeleteOldTokensAsync(dateLimit, cancellationToken);

        await unitOfWork.CommitAsync();

        logger.LogInformation(
            "Refresh token cleanup: {quantityRemoved} tokens removed (older than {dateLimit}).",
            quantityRemoved,
            dateLimit
        );
    }
}