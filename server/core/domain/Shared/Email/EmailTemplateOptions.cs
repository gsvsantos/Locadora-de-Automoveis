namespace LocadoraDeAutomoveis.Domain.Shared.Email;

public sealed class EmailTemplateOptions
{
    public string TemplatesFolderName { get; set; } = "Templates";
    public string DefaultLanguage { get; set; } = "pt-BR";
    public bool EnableNeutralLanguageFallback { get; set; } = true;
    public bool HtmlEncodeValues { get; set; } = true;
    public HashSet<string> RawPlaceholderKeys { get; set; } = new(StringComparer.Ordinal);
}
