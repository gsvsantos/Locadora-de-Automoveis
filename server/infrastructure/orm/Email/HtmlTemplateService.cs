using LocadoraDeAutomoveis.Application.Shared;

namespace LocadoraDeAutomoveis.Infrastructure.Email;

public class HtmlTemplateService : IEmailTemplateService
{
    private readonly string path;

    public HtmlTemplateService()
    {
        this.path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
    }

    public async Task<string> GetTemplateAsync(string templateName, Dictionary<string, string> placeholders)
    {
        string filePath = Path.Combine(this.path, $"{templateName}.html");

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Email template not found: {filePath}");
        }

        string templateContent = await File.ReadAllTextAsync(filePath);

        foreach (KeyValuePair<string, string> item in placeholders)
        {
            templateContent = templateContent.Replace($"{{{{{item.Key}}}}}", item.Value);
        }

        return templateContent;
    }
}