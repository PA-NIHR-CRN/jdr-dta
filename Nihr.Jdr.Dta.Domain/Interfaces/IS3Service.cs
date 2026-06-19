namespace Nihr.Jdr.Dta.Domain.Interfaces;

public interface IS3Service
{
    Task DownloadFileAsync(string s3Uri, string localPath);
    Task<Stream> GetFileStreamAsync(string s3Uri);
}