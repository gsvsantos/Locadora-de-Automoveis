namespace LocadoraDeAutomoveis.Application.Shared;

public interface IEmailTemplateService
{
    Task<string> GetTemplateAsync(string templateName, Dictionary<string, string> placeholders);
}