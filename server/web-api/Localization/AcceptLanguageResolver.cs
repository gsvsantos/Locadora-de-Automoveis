using LocadoraDeAutomoveis.Infrastructure.Localization;

namespace LocadoraDeAutomoveis.WebApi.Localization;

public sealed class AcceptLanguageResolver(IHttpContextAccessor httpContextAccessor)
{
    public string Resolve(string defaultLanguage = "en-US")
    {
        string? header = httpContextAccessor.HttpContext?.Request.Headers.AcceptLanguage.ToString();
        return LanguageCodes.ResolveFromHeader(header, defaultLanguage);
    }
}
