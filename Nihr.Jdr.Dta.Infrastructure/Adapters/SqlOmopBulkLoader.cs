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
    public async Task BulkLoadAsync(string schema, string tableName, string tsvPath)
    {
        logger.LogInformation(
            "Bulk loading [{Schema}].[{Table}] from {File}",
            schema, tableName, tsvPath);

        await using var connection = new SqlConnection(options.Value.ConnectionString);
        await connection.OpenAsync();

        using var reader = new StreamReader(tsvPath);
        using var csv = CreateCsvReader(reader);
        using var csvDataReader = new CsvDataReader(csv);
        using var dateFixer = new OmopDateFixingReader(csvDataReader);
        
        using var bulkCopy = new SqlBulkCopy(
            connection,
            SqlBulkCopyOptions.TableLock,
            null);
        
        bulkCopy.DestinationTableName = $"[{schema}].[{tableName}]";
        bulkCopy.BatchSize = 100_000;
        bulkCopy.BulkCopyTimeout = 0;

        await bulkCopy.WriteToServerAsync(dateFixer);
    }

    private static CsvReader CreateCsvReader(TextReader reader)
    {
        return new CsvReader(
            reader,
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
                HasHeaderRecord = true,
                IgnoreBlankLines = true,
                BadDataFound = null,
                MissingFieldFound = null,
                HeaderValidated = null
            });
    }
}