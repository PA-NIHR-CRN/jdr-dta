using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nihr.Jdr.Dta.CarrotCdm.Configuration;

namespace Nihr.Jdr.Dta.CarrotCdm.Execution;

public sealed class SourceTableExporter(
    ILogger<SourceTableExporter> logger,
    IConfiguration configuration,
    IOptions<CarrotCdmSettings> options)
{
    private readonly CarrotCdmSettings _options = options.Value;

    public async Task ExportAsync(CancellationToken cancellationToken = default)
    {
        var export = _options.SourceExport;

        var connectionString =
            configuration.GetConnectionString(export.ConnectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{export.ConnectionStringName}' not found.");

        Directory.CreateDirectory(_options.InputDirectory);

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        foreach (var table in export.Tables)
        {
            var outputPath = Path.Combine(
                _options.InputDirectory,
                table.OutputFile);

            logger.LogInformation(
                "Exporting {Schema}.{Table} → {Output}",
                table.Schema,
                table.Table,
                outputPath);

            await ExportTableAsync(
                connection,
                table,
                outputPath,
                cancellationToken);
        }
    }

    private static async Task ExportTableAsync(
        SqlConnection connection,
        SourceTableExport table,
        string outputPath,
        CancellationToken cancellationToken)
    {
        var sql = table.QueryOverride
            ?? $"SELECT * FROM [{table.Schema}].[{table.Table}]";

        await using var command = new SqlCommand(sql, connection);
        command.CommandTimeout = 0;

        await using var reader =
            await command.ExecuteReaderAsync(
                CommandBehavior.SequentialAccess,
                cancellationToken);

        await using var writer =
            new StreamWriter(outputPath, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        for (var i = 0; i < reader.FieldCount; i++)
        {
            if (i > 0) await writer.WriteAsync(',');
            await writer.WriteAsync(Escape(reader.GetName(i)));
        }
        await writer.WriteLineAsync();

        while (await reader.ReadAsync(cancellationToken))
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (i > 0) await writer.WriteAsync(',');

                if (reader.IsDBNull(i))
                {
                    continue;
                }

                var value = reader.GetValue(i);
                await writer.WriteAsync(FormatValue(value));
            }
            await writer.WriteLineAsync();
        }
    }
    
    private static string FormatValue(object value)
    {
        return value switch
        {
            DateTime dt =>
                Escape(dt.ToString("yyyy-MM-dd")),

            DateOnly d =>
                Escape(d.ToString("yyyy-MM-dd")),

            _ =>
                Escape(Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture)!)
        };
    }

    private static string Escape(string value)
    {
        if (value.Contains('"') || value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}