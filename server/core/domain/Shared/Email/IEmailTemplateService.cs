namespace LocadoraDeAutomoveis.Domain.Shared.Email;

public interface IEmailTemplateService
{
    Task<string> GetTemplateAsync(string templateName, Dictionary<string, string> placeholders, string? language = null, CancellationToken cancellationToken = default);
}