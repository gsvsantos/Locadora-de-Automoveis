using Hangfire;
using LocadoraDeAutomoveis.Application.Shared;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Shared.Email;

namespace LocadoraDeAutomoveis.Application.Auth.Services;

public class AuthEmailService(
    IEmailSender emailSender,
    IEmailTemplateService templateService
) : IAuthEmailService
{
    public async Task SendForgotPasswordEmailAsync(string email, string userName, string resetToken)
    {
        string encodedToken = Uri.EscapeDataString(resetToken);
        string encodedEmail = Uri.EscapeDataString(email);

        string link = $"http://localhost:4200/auth/reset-password?token={encodedToken}&email={encodedEmail}";

        Dictionary<string, string> placeholders = new()
        {
            {"UserName", userName },
            {"Link", link}
        };

        string body = await templateService.GetTemplateAsync("forget-password", placeholders);
        string subject = $"Password Recovery - LDA";

        BackgroundJob.Enqueue(() => emailSender.SendAsync(email, subject, body));
    }

    public async Task ScheduleBusinessRegisterWelcome(string email, string fullName)
    {
        Dictionary<string, string> placeholders = new()
        {
            {"UserFullName", fullName },
            {"LoginUrl", "http://localhost:4200/home"}
        };

        string body = await templateService.GetTemplateAsync("welcome-business", placeholders);
        string subject = $"Welcome Partner - LDA";

        BackgroundJob.Enqueue(() => emailSender.SendAsync(email, subject, body));
    }

    public async Task ScheduleBusinessGoogleLoginWelcome(string email, string fullName, string resetToken)
    {
        string encodedToken = Uri.EscapeDataString(resetToken);
        string encodedEmail = Uri.EscapeDataString(email);

        string link = $"http://localhost:4200/auth/reset-password?token={encodedToken}&email={encodedEmail}";

        Dictionary<string, string> placeholders = new()
        {
            {"UserFullName", fullName },
            {"ResetPasswordUrl", link}
        };

        string body = await templateService.GetTemplateAsync("welcome-business-google", placeholders);
        string subject = $"Google Registration - LDA";

        BackgroundJob.Enqueue(() => emailSender.SendAsync(email, subject, body));
    }

    public async Task ScheduleClientInvitation(string email, string fullName, string resetToken)
    {
        string encodedToken = Uri.EscapeDataString(resetToken);
        string encodedEmail = Uri.EscapeDataString(email);

        string link = $"http://localhost:4201/auth/reset-password?token={encodedToken}&email={encodedEmail}";

        Dictionary<string, string> placeholders = new()
        {
            {"ClientName", fullName },
            {"ResetPasswordUrl", link}
        };

        string body = await templateService.GetTemplateAsync("client-invitation", placeholders);
        string subject = "Welcome to the Family - LDA";

        // 4. Envia
        BackgroundJob.Enqueue(() => emailSender.SendAsync(email, subject, body));
    }
}
