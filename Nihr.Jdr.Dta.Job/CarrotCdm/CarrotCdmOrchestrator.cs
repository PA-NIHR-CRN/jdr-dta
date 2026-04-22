using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nihr.Jdr.Dta.Job.CarrotCdm.Execution;
using Nihr.Jdr.Dta.Job.Configuration;

namespace Nihr.Jdr.Dta.Job.CarrotCdm;

public sealed class CarrotCdmOrchestrator(
    ILogger<CarrotCdmOrchestrator> logger,
    SourceTableExporter exporter,
    CarrotTransformRunner transformRunner,
    OmopSchemaCreator schemaCreator,
    OmopBulkLoader bulkLoader,
    IOptions<CarrotCdmSettings> options)
{
    private readonly CarrotCdmSettings _options = options.Value;

    public async Task<int> RunAsync()
    {
        try
        {
            logger.LogInformation("Starting CarrotCDM job");

            await exporter.ExportAsync();

            var transformResult = await transformRunner.RunAsync();
            
            if (transformResult != 0)
            {
                logger.LogError("CarrotCDM transform failed with exit code {ExitCode}", transformResult);
                return transformResult;
            }

            await schemaCreator.CreateSchemaAsync(_options.DdlFile);

            await bulkLoader.ImportAsync();

            logger.LogInformation("CarrotCDM job completed successfully");
            return 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CarrotCDM job failed");
            return 1;
        }
    }
}