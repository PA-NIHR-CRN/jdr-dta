using Google.Apis.Auth.OAuth2;
using Google.Cloud.SecretManager.V1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NIHR.Infrastructure;
using NIHR.Infrastructure.Authentication;
using Nihr.Jdr.Dta.Domain.Interfaces;
using Nihr.Jdr.Dta.Infrastructure.Adapters;
using Nihr.Jdr.Dta.Infrastructure.DAL;
using Nihr.Jdr.Dta.Infrastructure.Settings;
using Nihr.Jdr.Dta.Job.CarrotCdm;
using Nihr.Jdr.Dta.Job.CarrotCdm.Execution;
using Nihr.Jdr.Dta.Job.Configuration;

namespace Nihr.Jdr.Dta.Job.Startup;

public static class DependencyInjection
{
    public static void ConfigureDependencyInjection(this HostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException(
                                   "Missing ConnectionStrings:DefaultConnection");

        builder.Services.AddDbContext<JdrDtaDbContext>(o => o.UseSqlServer(connectionString));

        builder.Services.GetSectionAndValidate<JdrDbSettings>(builder.Configuration);

        builder.Services.AddScoped(service =>
        {
            var googleSdkSettings = service.GetRequiredService<IOptions<JdrDbSettings>>().Value;

            var logger = service.GetRequiredService<ILogger<Program>>();
            logger.LogDebug("Initialising SecretManagerServiceClient with GCP credentials");

            var googleSdkCredential =
                CredentialFactory.FromJson<ServiceAccountCredential>(googleSdkSettings.JdrGcpSettings
                    .ServiceAccountKey);

            return new SecretManagerServiceClientBuilder { Credential = googleSdkCredential }.Build();
        });
        
        builder.Services.AddOptions<OmopSettings>()
            .Bind(builder.Configuration.GetSection(OmopSettings.SectionName))
            .ValidateOnStart();
        
        builder.Services.AddOptions<CarrotCdmSettings>()
            .Bind(builder.Configuration.GetSection(CarrotCdmSettings.SectionName))
            .ValidateOnStart();

        builder.Services.AddScoped<ICurrentUserIdProvider<int>, SimpleCurrentUserIdProvider<int>>();
        builder.Services.AddScoped<ICurrentUserIdAccessor<int>, SystemCurrentUserIdAccessor<int>>();
        builder.Services.AddScoped<IExternalJdrDbCredentialProvider, ExternalJdrDbCredentialProvider>();
        builder.Services.AddScoped<IExternalJdrDbRepository, ExternalJdrDbRepository>();
        builder.Services.AddScoped<IJdrDtaDbRepository, JdrDtaDbRepository>();
        builder.Services.AddScoped<JobRunner>();
        builder.Services.AddSingleton<SourceTableExporter>();
        builder.Services.AddSingleton<CarrotTransformRunner>();
        builder.Services.AddSingleton<OmopSchemaCreator>();
        builder.Services.AddSingleton<OmopBulkLoader>();
        builder.Services.AddSingleton<CarrotCdmOrchestrator>();
    }
}