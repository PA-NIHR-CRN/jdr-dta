using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nihr.Jdr.Dta.Domain.Interfaces;
using Nihr.Jdr.Dta.Infrastructure.Settings;
using System.Linq;

namespace Nihr.Jdr.Dta.Infrastructure.Adapters;

public sealed class SqlOmopSchemaRepository(
    ILogger<SqlOmopSchemaRepository> logger,
    IOptions<OmopSettings> options)
    : IOmopSchemaRepository
{
    public async Task DropSchemaAsync(string schema)
    {
        const string dropSql = """
            IF EXISTS (SELECT 1 FROM sys.schemas WHERE name = @SchemaName)
            BEGIN
                DECLARE @sql NVARCHAR(MAX) = N'';

                SELECT @sql +=
                    'DROP TABLE [' + s.name + '].[' + t.name + '];' + CHAR(10)
                FROM sys.tables t
                JOIN sys.schemas s ON t.schema_id = s.schema_id
                WHERE s.name = @SchemaName;

                EXEC sp_executesql @sql;
                EXEC('DROP SCHEMA [' + @SchemaName + ']');
            END
            """;

        logger.LogWarning(
            "Dropping schema [{Schema}] and all contained tables",
            schema);

        await using var connection = new SqlConnection(options.Value.ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = dropSql;
        command.Parameters.Add(new SqlParameter(
            "@SchemaName",
            System.Data.SqlDbType.NVarChar,
            128) { Value = schema });

        await command.ExecuteNonQueryAsync();
    }

    public async Task CreateSchemaAsync(string schema)
    {
        const string createSql = """
            IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = @SchemaName)
            BEGIN
                EXEC('CREATE SCHEMA [' + @SchemaName + ']')
            END
            """;

        await using var connection = new SqlConnection(options.Value.ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = createSql;
        command.Parameters.Add(new SqlParameter(
            "@SchemaName",
            System.Data.SqlDbType.NVarChar,
            128) { Value = schema });

        await command.ExecuteNonQueryAsync();
    }

    public async Task ExecuteDdlAsync(string ddlPath, string schema)
    {
        if (!File.Exists(ddlPath))
        {
            throw new FileNotFoundException(
                "OMOP DDL script not found",
                ddlPath);
        }

        logger.LogInformation(
            "Executing OMOP DDL script against schema [{Schema}]",
            schema);

        var ddlSql = await File.ReadAllTextAsync(ddlPath);

        ddlSql = ddlSql.Replace(
            "@cdmDatabaseSchema",
            schema,
            StringComparison.OrdinalIgnoreCase);

        await using var connection = new SqlConnection(options.Value.ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = ddlSql;
        command.CommandTimeout = 300;

        await command.ExecuteNonQueryAsync();
    }

    public async Task TruncateTablesAsync(string schema, IEnumerable<string> tableNames)
    {
        var tableList = tableNames.ToList();
        if (tableList.Count == 0) return;

        logger.LogInformation(
            "Truncating tables in schema [{Schema}]: {Tables}",
            schema,
            string.Join(", ", tableList));

        await using var connection = new SqlConnection(options.Value.ConnectionString);
        await connection.OpenAsync();

        foreach (var tableName in tableList)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = $"TRUNCATE TABLE [{schema}].[{tableName}]";
            await command.ExecuteNonQueryAsync();
        }
    }
}