using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Nihr.Jdr.Dta.Domain.Export;
using Nihr.Jdr.Dta.Domain.Interfaces;
using Nihr.Jdr.Dta.Infrastructure.Settings;

namespace Nihr.Jdr.Dta.Infrastructure.Adapters;

public sealed class SqlSourceTableExportRepository(
    IConfiguration configuration,
    IOptions<CarrotCdmSettings> options)
    : ISourceTableExportRepository
{
    private readonly string _connectionString =
        configuration.GetConnectionString(
            options.Value.SourceExport.ConnectionStringName)
        ?? throw new InvalidOperationException(
            $"Connection string '{options.Value.SourceExport.ConnectionStringName}' not found.");
    
    public async Task ExportAsync(SourceTableExport table, string outputPath)
    {
        var sql = table.QueryOverride ?? $"SELECT * FROM [{table.Schema}].[{table.Table}]";

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        command.CommandTimeout = 0;

        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

        await using var writer =
            new StreamWriter(outputPath, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        for (var i = 0; i < reader.FieldCount; i++)
        {
            if (i > 0) await writer.WriteAsync(',');
            await writer.WriteAsync(Escape(reader.GetName(i)));
        }
        await writer.WriteLineAsync();

        while (await reader.ReadAsync())
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (i > 0) await writer.WriteAsync(',');

                if (await reader.IsDBNullAsync(i))
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