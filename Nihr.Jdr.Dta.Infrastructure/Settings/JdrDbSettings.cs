using System.ComponentModel.DataAnnotations;

namespace Nihr.Jdr.Dta.Infrastructure.Settings;

public class JdrDbSettings : IValidatableObject
{
    public static string SectionName { get; set; } = "JdrDbSettings";

    [Required] public string IpAddress { get; set; } = null!;
    [Required] public string Username { get; set; } = null!;
    [Required] public string DatabaseName { get; set; } = null!;

    public JdrGcpSettings JdrGcpSettings { get; set; } = null!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(IpAddress))
        {
            yield return new ValidationResult("IpAddress is required", (IEnumerable<string>)
            [
                "IpAddress"
            ]);
        }

        if (string.IsNullOrWhiteSpace(Username))
        {
            yield return new ValidationResult("Username is required", (IEnumerable<string>)
            [
                "Username"
            ]);
        }

        if (string.IsNullOrWhiteSpace(DatabaseName))
        {
            yield return new ValidationResult("DatabaseName is required", (IEnumerable<string>)
            [
                "DatabaseName"
            ]);
        }
    }
}

public class JdrGcpSettings : IValidatableObject
{
    [Required(AllowEmptyStrings = false)] public string ServiceAccountKey { get; set; } = null!;
    [Required(AllowEmptyStrings = false)] public string FullyQualifiedSecretName { get; set; } = null!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(ServiceAccountKey))
        {
            yield return new ValidationResult("ServiceAccountKey is required", (IEnumerable<string>)
            [
                "ServiceAccountKey"
            ]);
        }

        if (string.IsNullOrWhiteSpace(FullyQualifiedSecretName))
        {
            yield return new ValidationResult("FullyQualifiedSecretName is required", (IEnumerable<string>)
            [
                "FullyQualifiedSecretName"
            ]);
        }
    }
}