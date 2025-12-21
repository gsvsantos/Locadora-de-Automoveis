namespace LocadoraDeAutomoveis.Infrastructure.S3;

public interface IR2FileStorageService
{
    Task<string> UploadAsync(Stream stream, string contentType, string key, CancellationToken cancellationToken = default);

    Task DeleteAsync(string key, CancellationToken cancellationToken = default);
}
