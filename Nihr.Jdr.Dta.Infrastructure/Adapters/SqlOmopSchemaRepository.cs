using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nihr.Jdr.Dta.Domain.Interfaces;

namespace Nihr.Jdr.Dta.Infrastructure.Adapters;

public sealed class SqlOmopSchemaRepository(
    ILogger<SqlOmopSchemaRepository> logger,
    IConfiguration configuration)
    : IOmopSchemaRepository
{
    private readonly string _connectionString =
        configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            $"Connection string DefaultConnection not found.");
    
    public async Task DropSchemaAsync(string schemaName)
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
            schemaName);

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = dropSql;
        command.Parameters.Add(new SqlParameter(
            "@SchemaName",
            System.Data.SqlDbType.NVarChar,
            128) { Value = schemaName });

        await command.ExecuteNonQueryAsync();
    }

    public async Task CreateSchemaAsync(string schemaName)
    {
        const string createSql = """
            IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = @SchemaName)
            BEGIN
                EXEC('CREATE SCHEMA [' + @SchemaName + ']')
            END
            """;

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = createSql;
        command.Parameters.Add(new SqlParameter(
            "@SchemaName",
            System.Data.SqlDbType.NVarChar,
            128) { Value = schemaName });

        await command.ExecuteNonQueryAsync();
    }

    public async Task ExecuteDdlAsync(string ddlScriptPath, string schemaName)
    {
        if (!File.Exists(ddlScriptPath))
        {
            throw new FileNotFoundException(
                "OMOP DDL script not found",
                ddlScriptPath);
        }

        logger.LogInformation(
            "Executing OMOP DDL script against schema [{Schema}]",
            schemaName);

        var ddlSql = await File.ReadAllTextAsync(ddlScriptPath);

        ddlSql = ddlSql.Replace(
            "@cdmDatabaseSchema",
            schemaName,
            StringComparison.OrdinalIgnoreCase);

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = ddlSql;
        command.CommandTimeout = 300;

        await command.ExecuteNonQueryAsync();
    }
}