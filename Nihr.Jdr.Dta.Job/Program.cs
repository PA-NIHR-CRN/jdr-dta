using Google.Apis.Auth.OAuth2;
using Google.Cloud.SecretManager.V1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Nihr.Jdr.Dta.Domain.Interfaces;
using Nihr.Jdr.Dta.Infrastructure.Adapters;
using Nihr.Jdr.Dta.Infrastructure.DAL;
using Nihr.Jdr.Dta.Infrastructure.Settings;

namespace Nihr.Jdr.Dta.Job;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        try
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.AddNihrConfiguration();
            builder.ConfigureNihrLogging();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                                   ?? throw new InvalidOperationException(
                                       "Missing ConnectionStrings:DefaultConnection");

            builder.Services.AddDbContext<JdrDtaDbContext>(o => o.UseSqlServer(connectionString));

            builder.Services.GetSectionAndValidate<JdrDbSettings>(builder.Configuration);

            builder.Services.AddScoped(service =>
            {
                var googleSdkSettings = service.GetRequiredService<IOptions<JdrDbSettings>>().Value;

                var googleSdkCredential =
                    CredentialFactory.FromJson<ServiceAccountCredential>(googleSdkSettings.JdrGcpSettings
                        .ServiceAccountKey);

                return new SecretManagerServiceClientBuilder { Credential = googleSdkCredential }.Build();
            });

            builder.Services.AddScoped<IExternalJdrDbCredentialProvider, ExternalJdrDbCredentialProvider>();
            builder.Services.AddScoped<IExternalJdrDbRepository, ExternalJdrDbRepository>();
            builder.Services.AddScoped<IJdrDtaDbRepository, JdrDtaDbRepository>();
            builder.Services.AddScoped<JobRunner>();

            var host = builder.Build();

            using var scope = host.Services.CreateScope();
            var job = scope.ServiceProvider.GetRequiredService<JobRunner>();
            var result = await job.RunAsync();

            return 0;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            return 1;
        }
    }
}