namespace Nihr.Jdr.Dta.Job.Configuration;

public class SourceExportSettings
{
    public string ConnectionStringName { get; init; } = null!;
    public IReadOnlyList<SourceTableExport> Tables { get; init; } = [];
}