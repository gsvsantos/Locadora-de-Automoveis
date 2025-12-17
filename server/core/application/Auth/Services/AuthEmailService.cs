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
    public async Task ScheduleGoogleLoginWelcome(User user)
    {
        Dictionary<string, string> placeholders = new()
        {
            {"UserFullName", user.FullName },
            {"CreatePasswordUrl", "http://localhost:4200/profile/create-password"}
        };

        string body = await templateService.GetTemplateAsync("welcome-business-google", placeholders);
        string subject = $"Google Registration - LDA";

        BackgroundJob.Enqueue(() => emailSender.SendAsync(user.Email!, subject, body));
    }
}
