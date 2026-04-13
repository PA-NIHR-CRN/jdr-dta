namespace Nihr.Jdr.Dta.CarrotCdm.Configuration;

public class SourceTableExport
{
    public string Schema { get; init; } = "dbo";
    public string Table { get; init; } = null!;
    public string OutputFile { get; init; } = null!;
    public string? QueryOverride { get; init; }
}