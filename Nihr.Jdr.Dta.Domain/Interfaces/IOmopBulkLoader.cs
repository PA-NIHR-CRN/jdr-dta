namespace Nihr.Jdr.Dta.Domain.Interfaces;

public interface IOmopBulkLoader
{
    Task BulkLoadAsync(string schema, string tableName, string tsvPath);
}