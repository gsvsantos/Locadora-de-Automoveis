namespace LocadoraDeAutomoveis.Infrastructure.Email;

public sealed class EmailTemplateOptions
{
    public string TemplatesFolderName { get; set; } = "Templates";
    public string DefaultLanguage { get; set; } = "en-US";
    public bool EnableNeutralLanguageFallback { get; set; } = true;
    public bool HtmlEncodeValues { get; set; } = true;
    public HashSet<string> RawPlaceholderKeys { get; set; } = new(StringComparer.Ordinal);
}
