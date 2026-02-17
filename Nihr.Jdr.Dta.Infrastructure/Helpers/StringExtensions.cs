namespace Nihr.Jdr.Dta.Infrastructure.Helpers;

public static class StringExtensions
{
    public static string? GetNonEmptyString(this string? input)
    {
        return !string.IsNullOrWhiteSpace(input) ? input : null;
    }
}