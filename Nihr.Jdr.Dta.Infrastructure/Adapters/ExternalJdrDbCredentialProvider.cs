using Google.Cloud.SecretManager.V1;
using Microsoft.Extensions.Options;
using Nihr.Jdr.Dta.Domain.Interfaces;
using Nihr.Jdr.Dta.Infrastructure.Settings;

namespace Nihr.Jdr.Dta.Infrastructure.Adapters;

public class ExternalJdrDbCredentialProvider(
    SecretManagerServiceClient secretManagerServiceClient,
    IOptions<JdrDbSettings> jdrDbSettings)
    : IExternalJdrDbCredentialProvider
{
    private const string JdrDbConnectionString = "server={0};user={1};password={2};database={3};port=3306";

    private async Task<string> GetSecretAsync(CancellationToken cancellationToken)
    {
        var secret = await secretManagerServiceClient.AccessSecretVersionAsync(new AccessSecretVersionRequest()
        {
            Name = jdrDbSettings.Value.JdrGcpSettings.FullyQualifiedSecretName
        }, cancellationToken);

        return secret.Payload.Data.ToStringUtf8();
    }

    public async Task<string> GetConnectionString(CancellationToken cancellationToken)
    {
        var dbPassword = await GetSecretAsync(cancellationToken);
        return string.Format(JdrDbConnectionString, jdrDbSettings.Value.IpAddress, jdrDbSettings.Value.Username,
            dbPassword, jdrDbSettings.Value.DatabaseName);
    }
}