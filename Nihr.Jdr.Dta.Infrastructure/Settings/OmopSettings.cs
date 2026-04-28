using System.ComponentModel.DataAnnotations;

namespace Nihr.Jdr.Dta.Infrastructure.Settings;

public class OmopSettings : IValidatableObject
{
    [Required] public string SchemaName { get; set; } = null!;
    [Required] public string ConnectionString { get; set; } = null!;
    public List<string> SkipImportTableNames { get; init; } = [];
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(SchemaName))
        {
            yield return new ValidationResult("SchemaName is required", (IEnumerable<string>)
            [
                "SchemaName"
            ]);
        }
        
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            yield return new ValidationResult("ConnectionString is required", (IEnumerable<string>)
            [
                "ConnectionString"
            ]);
        }
    }
}