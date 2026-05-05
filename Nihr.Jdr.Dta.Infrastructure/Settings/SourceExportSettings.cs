using Nihr.Jdr.Dta.Domain.Export;

namespace Nihr.Jdr.Dta.Infrastructure.Settings;

public class SourceExportSettings
{
    public string ConnectionStringName { get; init; } = null!;
    public IReadOnlyList<SourceTableExport> Tables { get; init; } = [];
}