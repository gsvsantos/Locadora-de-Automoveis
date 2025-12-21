using LocadoraDeAutomoveis.Domain.Shared.Email;
using Microsoft.Extensions.Hosting;
using System.Text.RegularExpressions;

namespace LocadoraDeAutomoveis.Infrastructure.Email;

public sealed class HtmlTemplateService : IEmailTemplateService
{
    private static readonly Regex PlaceholderRegex =
        new(@"{{\s*(?<key>[A-Za-z0-9_]+)\s*}}", RegexOptions.Compiled);

    private readonly string templatesPath;
    private readonly string defaultLanguage = "en-US";
    private readonly bool enableNeutralLanguageFallback = true;

    public HtmlTemplateService(IHostEnvironment env)
    {
        this.templatesPath = Path.Combine(env.ContentRootPath, "Templates");
    }

    public async Task<string> GetTemplateAsync(
        string templateName,
        Dictionary<string, string> placeholders,
        string? language = null,
        CancellationToken cancellationToken = default)
    {
        string lang = NormalizeLanguage(language) ?? this.defaultLanguage;

        string filePath = ResolveTemplatePath(templateName, lang);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Email template not found: {filePath}");
        }

        string templateContent = await File.ReadAllTextAsync(filePath, cancellationToken);

        ValidatePlaceholders(templateName, filePath, templateContent, placeholders);

        string rendered = PlaceholderRegex.Replace(templateContent, match =>
        {
            string key = match.Groups["key"].Value;
            return placeholders.TryGetValue(key, out string? value) ? value : match.Value;
        });

        EnsureNoUnresolved(templateName, filePath, rendered);

        return rendered;
    }

    private string ResolveTemplatePath(string templateName, string lang)
    {
        List<string> candidates =
        [
            Path.Combine(this.templatesPath, $"{templateName}.{lang}.html")
        ];

        if (this.enableNeutralLanguageFallback)
        {
            string? neutral = TryGetNeutralLanguage(lang);
            if (!string.IsNullOrWhiteSpace(neutral))
            {
                candidates.Add(Path.Combine(this.templatesPath, $"{templateName}.{neutral}.html"));
            }
        }

        candidates.Add(Path.Combine(this.templatesPath, $"{templateName}.{this.defaultLanguage}.html"));
        candidates.Add(Path.Combine(this.templatesPath, $"{templateName}.html"));

        foreach (string c in candidates)
        {
            if (File.Exists(c))
            {
                return c;
            }
        }

        return candidates[0];
    }

    private static void ValidatePlaceholders(
        string templateName,
        string filePath,
        string templateContent,
        Dictionary<string, string> placeholders)
    {
        List<string> required = PlaceholderRegex.Matches(templateContent)
            .Select(m => m.Groups["key"].Value)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        List<string> missing = required.Where(k => !placeholders.ContainsKey(k)).ToList();

        if (missing.Count > 0)
        {
            throw new InvalidOperationException(
                $"Template '{templateName}' is missing placeholders: {string.Join(", ", missing)}. File: {filePath}");
        }
    }

    private static void EnsureNoUnresolved(string templateName, string filePath, string rendered)
    {
        List<string> unresolved = PlaceholderRegex.Matches(rendered)
            .Select(m => m.Groups["key"].Value)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (unresolved.Count > 0)
        {
            throw new InvalidOperationException(
                $"Template '{templateName}' has unresolved placeholders: {string.Join(", ", unresolved)}. File: {filePath}");
        }
    }

    private static string? NormalizeLanguage(string? language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return null;
        }

        return language.Trim().Replace('_', '-');
    }

    private static string? TryGetNeutralLanguage(string lang)
    {
        int dash = lang.IndexOf('-', StringComparison.Ordinal);
        return dash > 0 ? lang[..dash] : null;
    }
}
