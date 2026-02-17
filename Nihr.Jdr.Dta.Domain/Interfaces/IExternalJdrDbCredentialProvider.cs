namespace Nihr.Jdr.Dta.Domain.Interfaces;

public interface IExternalJdrDbCredentialProvider
{
    Task<string> GetConnectionString(CancellationToken cancellationToken);
}