namespace CustomizableForms.Domain.DTOs;

public record UserPreferences
{
    public string PrefLang { get; init; }
    public string PrefTheme { get; init; }
}