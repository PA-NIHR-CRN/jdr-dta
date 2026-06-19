using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nihr.Jdr.Dta.Domain.Interfaces;
using Nihr.Jdr.Dta.Infrastructure.DataReaders;
using Nihr.Jdr.Dta.Infrastructure.Settings;

namespace Nihr.Jdr.Dta.Infrastructure.Adapters;

public sealed class SqlOmopBulkLoader(
    ILogger<SqlOmopBulkLoader> logger,
    IOptions<OmopSettings> options)
    : IOmopBulkLoader
{
    public async Task BulkLoadFromFileAsync(string schema, string tableName, string filePath, string delimiter = "\t")
    {
        logger.LogInformation(
            "Bulk loading [{Schema}].[{Table}] from {File} (Delimiter: '{Delimiter}')",
            schema, tableName, filePath, delimiter == "\t" ? "\\t" : delimiter);

        await using var connection = new SqlConnection(options.Value.ConnectionString);
        await connection.OpenAsync();

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = delimiter,
            HasHeaderRecord = true,
            IgnoreBlankLines = true,
            BadDataFound = null,
            MissingFieldFound = null,
            HeaderValidated = null
        });

        using var csvDataReader = new CsvDataReader(csv);
        using var dateFixer = new OmopDateFixingReader(csvDataReader);

        using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, null);
        bulkCopy.DestinationTableName = $"[{schema}].[{tableName}]";
        bulkCopy.BatchSize = 100_000;
        bulkCopy.BulkCopyTimeout = 0;

        await bulkCopy.WriteToServerAsync(dateFixer);
    }

    public async Task BulkLoadFromStreamAsync(string schema, string tableName, TextReader reader, string delimiter = "\t")
    {
        await using var connection = new SqlConnection(options.Value.ConnectionString);
        await connection.OpenAsync();

        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = delimiter,
            HasHeaderRecord = true
        });
    
        using var csvDataReader = new CsvDataReader(csv);
        using var dateFixer = new OmopDateFixingReader(csvDataReader);
    
        using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, null)
        {
            DestinationTableName = $"[{schema}].[{tableName}]",
            BatchSize = 100_000
        };

        await bulkCopy.WriteToServerAsync(dateFixer);
    }
}