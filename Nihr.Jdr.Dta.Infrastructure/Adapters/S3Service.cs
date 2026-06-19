using Amazon.S3;
using Amazon.S3.Transfer;
using Nihr.Jdr.Dta.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Nihr.Jdr.Dta.Infrastructure.Adapters;

public class S3Service(IAmazonS3 s3Client, ILogger<S3Service> logger) : IS3Service
{
    public async Task DownloadFileAsync(string s3Uri, string localPath)
    {
        var (bucket, key) = ParseS3Uri(s3Uri);
        
        logger.LogInformation("Downloading s3://{Bucket}/{Key} to {LocalPath}", bucket, key, localPath);

        using var fileTransferUtility = new TransferUtility(s3Client);
        await fileTransferUtility.DownloadAsync(localPath, bucket, key);
    }
    
    public async Task<Stream> GetFileStreamAsync(string s3Uri)
    {
        var (bucket, key) = ParseS3Uri(s3Uri);
        var response = await s3Client.GetObjectAsync(bucket, key);
        return response.ResponseStream;
    }

    private static (string Bucket, string Key) ParseS3Uri(string s3Uri)
    {
        if (!s3Uri.StartsWith("s3://", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Invalid S3 URI. Must start with s3://", nameof(s3Uri));
        }

        var uri = new Uri(s3Uri);
        var bucket = uri.Host;
        var key = uri.AbsolutePath.TrimStart('/');

        return (bucket, key);
    }
}