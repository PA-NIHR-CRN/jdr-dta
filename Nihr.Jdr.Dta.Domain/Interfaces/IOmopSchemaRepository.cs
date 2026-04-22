namespace Nihr.Jdr.Dta.Domain.Interfaces;

public interface IOmopSchemaRepository
{
    Task DropSchemaAsync(string schema);
    Task CreateSchemaAsync(string schema);
    Task ExecuteDdlAsync(string ddlPath, string schema);
}