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
    public async Task ScheduleBusinessRegisterWelcome(User user)
    {
        Dictionary<string, string> placeholders = new()
        {
            {"UserFullName", user.FullName },
            {"LoginUrl", "http://localhost:4200/home"}
        };

        string body = await templateService.GetTemplateAsync("welcome-business", placeholders);
        string subject = $"Welcome Partner - LDA";

        BackgroundJob.Enqueue(() => emailSender.SendAsync(user.Email!, subject, body));
    }

    public async Task ScheduleBusinessGoogleLoginWelcome(User user, string resetToken)
    {
        string encodedToken = Uri.EscapeDataString(resetToken);
        string encodedEmail = Uri.EscapeDataString(user.Email!);

        string link = $"http://localhost:4200/auth/reset-password?token={encodedToken}&email={encodedEmail}";

        Dictionary<string, string> placeholders = new()
        {
            {"UserFullName", user.FullName },
            {"ResetPasswordUrl", link}
        };

        string body = await templateService.GetTemplateAsync("welcome-business-google", placeholders);
        string subject = $"Google Registration - LDA";

        BackgroundJob.Enqueue(() => emailSender.SendAsync(user.Email!, subject, body));
    }

    public async Task ScheduleClientInvitation(User user, string resetToken)
    {
        string encodedToken = Uri.EscapeDataString(resetToken);
        string encodedEmail = Uri.EscapeDataString(user.Email!);

        string link = $"http://localhost:4201/auth/reset-password?token={encodedToken}&email={encodedEmail}";

        Dictionary<string, string> placeholders = new()
        {
            {"ClientName", user.FullName },
            {"ResetPasswordUrl", link}
        };

        string body = await templateService.GetTemplateAsync("client-invitation", placeholders);
        string subject = "Welcome to the Family - LDA";

        // 4. Envia
        BackgroundJob.Enqueue(() => emailSender.SendAsync(user.Email!, subject, body));
    }
}
