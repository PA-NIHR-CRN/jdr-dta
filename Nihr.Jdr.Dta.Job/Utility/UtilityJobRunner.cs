using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nihr.Jdr.Dta.Domain.Interfaces;
using Nihr.Jdr.Dta.Infrastructure.Settings;

namespace Nihr.Jdr.Dta.Job.Utility;

public class UtilityJobRunner(
    ILogger<UtilityJobRunner> logger,
    IOmopSchemaRepository omopSchemaRepository,
    IOmopBulkLoader omopBulkLoader,
    IOptions<CarrotCdmSettings> carrotOptions,
    IOptions<OmopSettings> omopOptions,
    IS3Service s3Service)
{
    private static readonly string[] VocabularyFiles = 
    [
        "CONCEPT.csv", "VOCABULARY.csv", "DOMAIN.csv", "CONCEPT_CLASS.csv", 
        "RELATIONSHIP.csv", "CONCEPT_RELATIONSHIP.csv", "CONCEPT_SYNONYM.csv", 
        "CONCEPT_ANCESTOR.csv", "DRUG_STRENGTH.csv"
    ];

    public async Task<int> DeployDdlAsync()
    {
        logger.LogInformation("Starting DDL deployment...");
        var schema = omopOptions.Value.SchemaName;
        var ddlPath = carrotOptions.Value.DdlFile;

        await omopSchemaRepository.CreateSchemaAsync(schema);
        await omopSchemaRepository.ExecuteDdlAsync(ddlPath, schema);
        
        logger.LogInformation("DDL deployed successfully to schema {Schema}", schema);
        return 0;
    }

    public async Task<int> LoadReferenceDataAsync(string[] args)
    {
        if (args.Length < 1)
        {
            logger.LogError("Usage: --load-ref-data s3://bucket/path/to/prefix/");
            return 1;
        }

        var s3Path = args[0].TrimEnd('/');
        var schema = omopOptions.Value.SchemaName;

        foreach (var fileName in VocabularyFiles)
        {
            var s3Url = $"{s3Path}/{fileName}";
            var tableName = Path.GetFileNameWithoutExtension(fileName);
            var localPath = Path.Combine(Path.GetTempPath(), fileName);

            try
            {
                // Download from S3 using the new service
                await s3Service.DownloadFileAsync(s3Url, localPath);

                if (!File.Exists(localPath))
                {
                    logger.LogError("Downloaded file not found at {LocalPath} for {S3Url}", localPath, s3Url);
                    return 1;
                }

                await omopBulkLoader.BulkLoadFromFileAsync(schema, tableName, localPath, "\t");
                logger.LogInformation("Successfully loaded {Table}", tableName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load {Table} from {S3Url}", tableName, s3Url);
                return 1;
            }
            finally
            {
                try
                {
                    if (File.Exists(localPath))
                    {
                        File.Delete(localPath);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to delete temp file {LocalPath}", localPath);
                }
            }
        }

        return 0;
    }
}