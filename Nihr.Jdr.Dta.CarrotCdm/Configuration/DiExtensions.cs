using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nihr.Jdr.Dta.CarrotCdm.Execution;

namespace Nihr.Jdr.Dta.CarrotCdm.Configuration;

public static class DiExtensions
{
    public static IServiceCollection AddCarrotCdm(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<OmopSettings>()
            .Bind(configuration.GetSection(OmopSettings.SectionName))
            .ValidateOnStart();
        
        services.AddOptions<CarrotCdmSettings>()
            .Bind(configuration.GetSection(CarrotCdmSettings.SectionName))
            .ValidateOnStart();

        services.AddSingleton<SourceTableExporter>();
        services.AddSingleton<CarrotTransformRunner>();
        services.AddSingleton<OmopSchemaCreator>();
        services.AddSingleton<OmopBulkLoader>();

        services.AddSingleton<CarrotCdmJob>();

        return services;
    }
}