using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nihr.Jdr.Dta.Domain.Interfaces;
using Nihr.Jdr.Dta.Infrastructure.Settings;

namespace Nihr.Jdr.Dta.Job.CarrotCdm;

public sealed class CarrotCdmTransformJob(
    ILogger<CarrotCdmTransformJob> logger,
    ISourceTableExportRepository sourceExportRepository,
    IOmopSchemaRepository omopSchemaRepository,
    IOmopBulkLoader omopBulkLoader,
    CarrotTransformRunner transformRunner,
    IOptions<CarrotCdmSettings> carrotOptions,
    IOptions<OmopSettings> omopOptions)
{
    private readonly CarrotCdmSettings _carrot = carrotOptions.Value;
    private readonly OmopSettings _omop = omopOptions.Value;

    public async Task<int> RunAsync()
    {
        try
        {
            logger.LogInformation("Starting CarrotCDM transform job");

            await ExportSourceTablesAsync();

            var transformExitCode = await transformRunner.RunAsync();
            if (transformExitCode != 0)
            {
                logger.LogError(
                    "CarrotCDM transform failed with exit code {ExitCode}",
                    transformExitCode);

                return transformExitCode;
            }

            await RebuildOmopSchemaAsync();
            await ImportOmopDataAsync();

            logger.LogInformation("CarrotCDM transform job completed successfully");
            return 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CarrotCDM transform job failed");
            return 1;
        }
    }

    private async Task ExportSourceTablesAsync()
    {
        var exportConfig = _carrot.SourceExport;

        Directory.CreateDirectory(_carrot.InputDirectory);

        foreach (var table in exportConfig.Tables)
        {
            var outputPath = Path.Combine(_carrot.InputDirectory, table.OutputFile);

            logger.LogInformation(
                "Exporting {Schema}.{Table} → {Output}",
                table.Schema,
                table.Table,
                outputPath);

            await sourceExportRepository.ExportAsync(table, outputPath);
        }
    }

    private async Task RebuildOmopSchemaAsync()
    {
        logger.LogInformation("Rebuilding OMOP schema {Schema}", _omop.SchemaName);

        await omopSchemaRepository.DropSchemaAsync(_omop.SchemaName);

        await omopSchemaRepository.CreateSchemaAsync(_omop.SchemaName);

        await omopSchemaRepository.ExecuteDdlAsync(_carrot.DdlFile, _omop.SchemaName);
    }

    private async Task ImportOmopDataAsync()
    {
        logger.LogInformation("Importing OMOP TSV data");

        var tsvFiles = Directory.GetFiles(
            _carrot.OutputDirectory,
            "*.tsv");

        if (tsvFiles.Length == 0)
        {
            throw new InvalidOperationException(
                "No OMOP TSV files found to import");
        }

        foreach (var file in tsvFiles)
        {
            var tableName = Path.GetFileNameWithoutExtension(file);

            if (_omop.SkipImportTableNames.Contains(tableName, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            logger.LogInformation(
                "Bulk loading {Schema}.{Table} from {File}",
                _omop.SchemaName,
                tableName,
                file);

            await omopBulkLoader.BulkLoadAsync(_omop.SchemaName, tableName, file);
        }
    }
}