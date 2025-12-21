namespace LocadoraDeAutomoveis.Infrastructure.Localization;

public static class LanguageCodes
{
    private static readonly HashSet<string> Supported = new(StringComparer.OrdinalIgnoreCase)
    {
        "en-US", "pt-BR", "es-ES"
    };

    public static bool IsSupported(string language)
    {
        return Supported.Contains(language);
    }

    public static string ResolveFromHeader(string? acceptLanguageHeader, string defaultLanguage)
    {
        if (string.IsNullOrWhiteSpace(acceptLanguageHeader))
        {
            return defaultLanguage;
        }

        string first = acceptLanguageHeader.Split(',')[0].Trim();
        string normalized = first.Replace('_', '-');

        return IsSupported(normalized) ? normalized : defaultLanguage;
    }
}
