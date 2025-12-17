using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace LocadoraDeVeiculos.Infrastructure.S3;

public sealed class R2FileStorageService(IAmazonS3 s3, IOptions<CloudflareR2Options> options)
{
    private readonly CloudflareR2Options options = options.Value;

    public async Task<string> UploadAsync(
        Stream stream,
        string contentType,
        string key,
        CancellationToken cancellationToken = default
    )
    {
        PutObjectRequest putRequest = new()
        {
            BucketName = this.options.BucketName,
            Key = key,
            InputStream = stream,
            ContentType = contentType,
            DisablePayloadSigning = true,
            DisableDefaultChecksumValidation = true,
        };

        await s3.PutObjectAsync(putRequest, cancellationToken);

        return key;
    }

    public async Task DeleteAsync(
        string key,
        CancellationToken cancellationToken = default
    )
    {
        DeleteObjectRequest deleteRequest = new()
        {
            BucketName = this.options.BucketName,
            Key = key,
        };

        await s3.DeleteObjectAsync(deleteRequest, cancellationToken);
    }
}
