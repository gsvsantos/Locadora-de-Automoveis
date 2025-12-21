using LocadoraDeAutomoveis.Domain.Shared.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace LocadoraDeAutomoveis.Infrastructure.Email;

public class SmtpEmailSender(
    IOptions<MailSettings> mailSettings,
    ILogger<SmtpEmailSender> logger
) : IEmailSender
{
    private readonly MailSettings settings = mailSettings.Value;

    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        try
        {
            MimeMessage message = new();
            message.From.Add(new MailboxAddress(this.settings.SenderName, this.settings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            BodyBuilder builder = new() { HtmlBody = htmlBody };
            message.Body = builder.ToMessageBody();

            using SmtpClient client = new();

            await client.ConnectAsync(this.settings.Host, this.settings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(this.settings.UserName, this.settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            logger.LogInformation("Email sent successfully to {Email}", to);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {Email}", to);
            throw;
        }
    }
}