namespace LocadoraDeAutomoveis.Domain.Shared.Email;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string htmlBody);
}