namespace Nihr.Jdr.Dta.Domain.Interfaces;

public interface IOmopBulkLoader
{
    Task BulkLoadFromFileAsync(string schema, string tableName, string tsvPath, string delimiter = "\t");
    Task BulkLoadFromStreamAsync(string schema, string tableName, TextReader reader, string delimiter = "\t");
}