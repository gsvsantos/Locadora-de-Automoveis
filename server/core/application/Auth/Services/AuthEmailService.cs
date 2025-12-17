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

    public async Task ScheduleBusinessGoogleLoginWelcome(User user)
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
