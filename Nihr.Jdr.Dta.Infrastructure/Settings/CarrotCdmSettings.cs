using System.ComponentModel.DataAnnotations;

namespace Nihr.Jdr.Dta.Infrastructure.Settings;

public class CarrotCdmSettings : IValidatableObject
{
    public static string SectionName { get; set; } = "CarrotCdm";

    [Required] public string InputDirectory { get; set; } = null!;
    [Required] public string RulesFile { get; set; } = null!;
    [Required] public string PersonTable { get; set; } = null!;
    [Required] public string OutputDirectory { get; set; } = null!;
    [Required] public string DdlFile { get; set; } = null!;
    [Required] public string ConfigFile { get; set; } = null!;
    [Required] public SourceExportSettings SourceExport { get; init; } = null!;
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(InputDirectory))
        {
            yield return new ValidationResult("InputDirectory is required", (IEnumerable<string>)
            [
                "InputDirectory"
            ]);
        }
        
        if (string.IsNullOrWhiteSpace(RulesFile))
        {
            yield return new ValidationResult("RulesFile is required", (IEnumerable<string>)
            [
                "RulesFile"
            ]);
        }
        
        if (string.IsNullOrWhiteSpace(PersonTable))
        {
            yield return new ValidationResult("PersonTable is required", (IEnumerable<string>)
            [
                "PersonTable"
            ]);
        }
        
        if (string.IsNullOrWhiteSpace(OutputDirectory))
        {
            yield return new ValidationResult("OutputDirectory is required", (IEnumerable<string>)
            [
                "OutputDirectory"
            ]);
        }
        
        if (string.IsNullOrWhiteSpace(DdlFile))
        {
            yield return new ValidationResult("DdlFile is required", (IEnumerable<string>)
            [
                "DdlFile"
            ]);
        }
        
        if (string.IsNullOrWhiteSpace(ConfigFile))
        {
            yield return new ValidationResult("ConfigFile is required", (IEnumerable<string>)
            [
                "ConfigFile"
            ]);
        }
    }
}