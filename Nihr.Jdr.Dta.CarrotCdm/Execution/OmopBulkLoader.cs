using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nihr.Jdr.Dta.CarrotCdm.Configuration;
using Nihr.Jdr.Dta.CarrotCdm.Readers;

namespace Nihr.Jdr.Dta.CarrotCdm.Execution;

public sealed class OmopBulkLoader(
    ILogger<OmopBulkLoader> logger,
    IOptions<CarrotCdmSettings> carrotCdmSettings,
    IOptions<OmopSettings> omopSettings)
{
    private static readonly HashSet<string> SkippedTableNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "summary_mapstream",
        "person_ids"
    };

    private readonly CarrotCdmSettings _carrotCdmSettings = carrotCdmSettings.Value;
    private readonly OmopSettings _omopSettings = omopSettings.Value;

    public async Task ImportAsync()
    {
        var tsvFiles = GetTsvFiles();

        await using var connection = new SqlConnection(_omopSettings.ConnectionString);
        await connection.OpenAsync();

        foreach (var file in tsvFiles)
        {
            if (ShouldSkipFile(file))
            {
                continue;
            }

            var tableName = Path.GetFileNameWithoutExtension(file);

            logger.LogInformation(
                "Bulk loading table {Schema}.{Table} from {File}",
                _omopSettings.SchemaName, tableName, file);

            await BulkLoadTableAsync(connection, tableName, file);
        }

        logger.LogInformation("OMOP TSV bulk load completed successfully");
    }

    private string[] GetTsvFiles()
    {
        if (!Directory.Exists(_carrotCdmSettings.OutputDirectory))
        {
            throw new DirectoryNotFoundException(
                $"Output directory not found: {_carrotCdmSettings.OutputDirectory}");
        }

        var tsvFiles = Directory.GetFiles(_carrotCdmSettings.OutputDirectory, "*.tsv");

        return tsvFiles.Length == 0 ? throw new InvalidOperationException("No OMOP TSV files found to import") : tsvFiles;
    }

    private static bool ShouldSkipFile(string filePath)
    {
        var tableName = Path.GetFileNameWithoutExtension(filePath);
        return SkippedTableNames.Contains(tableName);
    }

    private async Task BulkLoadTableAsync(
        SqlConnection connection,
        string tableName,
        string tsvPath)
    {
        using var reader = new StreamReader(tsvPath);
        using var csv = CreateCsvReader(reader);
        using var csvDataReader = new CsvDataReader(csv);
        using var nullFixer = new NullNormalizingDataReader(csvDataReader);
        using var dateFixer = new OmopObservationDateFixingReader(nullFixer);
        using var bulkCopy = CreateBulkCopy(connection, tableName);

        await bulkCopy.WriteToServerAsync(dateFixer);
    }

    private static CsvReader CreateCsvReader(TextReader reader)
    {
        var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "\t",
            HasHeaderRecord = true,
            IgnoreBlankLines = true,
            BadDataFound = null,
            MissingFieldFound = null,
            HeaderValidated = null,
            DetectDelimiter = false,
            TrimOptions = TrimOptions.None
        };

        return new CsvReader(reader, configuration);
    }

    private SqlBulkCopy CreateBulkCopy(SqlConnection connection, string tableName)
    {
        var bulkCopy = new SqlBulkCopy(
            connection,
            SqlBulkCopyOptions.TableLock,
            null)
        {
            DestinationTableName = $"[{_omopSettings.SchemaName}].[{tableName}]",
            BatchSize = 100_000,
            BulkCopyTimeout = 0
        };

        return bulkCopy;
    }
}