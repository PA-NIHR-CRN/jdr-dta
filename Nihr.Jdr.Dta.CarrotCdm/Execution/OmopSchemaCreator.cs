using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nihr.Jdr.Dta.CarrotCdm.Configuration;

namespace Nihr.Jdr.Dta.CarrotCdm.Execution;

public sealed class OmopSchemaCreator(
    ILogger<OmopSchemaCreator> logger,
    IOptions<OmopSettings> omopSettings)
{
    private readonly OmopSettings _omopSettings = omopSettings.Value;

    public async Task CreateSchemaAsync(string ddlScriptPath)
    {
        logger.LogInformation("Creating OMOP schema {Schema}", _omopSettings.SchemaName);

        await using var connection = new SqlConnection(_omopSettings.ConnectionString);
        await connection.OpenAsync();

        await DropSchemaIfExistsAsync(connection);
        await EnsureSchemaExistsAsync(connection);
        await ExecuteDdlAsync(connection, ddlScriptPath);

        logger.LogInformation("OMOP schema {Schema} created successfully", _omopSettings.SchemaName);
    }
    
    private async Task DropSchemaIfExistsAsync(SqlConnection connection)
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
            "Dropping existing schema [{Schema}] (all tables will be deleted)",
            _omopSettings.SchemaName);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = dropSql;
        cmd.Parameters.Add(new SqlParameter("@SchemaName", System.Data.SqlDbType.NVarChar, 128)
        {
            Value = _omopSettings.SchemaName
        });

        await cmd.ExecuteNonQueryAsync();
    }
    
    private async Task EnsureSchemaExistsAsync(SqlConnection connection)
    {
        const string createSchemaSql = """
                                       IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = @SchemaName)
                                       BEGIN
                                           EXEC('CREATE SCHEMA [' + @SchemaName + ']')
                                       END
                                       """;

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = createSchemaSql;
        cmd.Parameters.Add(new SqlParameter("@SchemaName", System.Data.SqlDbType.NVarChar, 128)
        {
            Value = _omopSettings.SchemaName
        });

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task ExecuteDdlAsync(SqlConnection connection, string ddlScriptPath)
    {
        logger.LogInformation("Executing OMOP DDL");

        var ddlSql = await File.ReadAllTextAsync(ddlScriptPath);
        ddlSql = ddlSql.Replace("@cdmDatabaseSchema", _omopSettings.SchemaName);

        await using var ddlCmd = connection.CreateCommand();
        ddlCmd.CommandTimeout = 300;
        ddlCmd.CommandText = ddlSql;

        await ddlCmd.ExecuteNonQueryAsync();
    }
}