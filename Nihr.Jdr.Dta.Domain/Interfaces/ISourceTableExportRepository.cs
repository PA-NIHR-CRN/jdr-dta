using Nihr.Jdr.Dta.Domain.Export;

namespace Nihr.Jdr.Dta.Domain.Interfaces;

public interface ISourceTableExportRepository
{
    Task ExportAsync(SourceTableExport table, string outputPath);
}