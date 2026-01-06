using Hangfire;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Domain.Shared.Email;
using Microsoft.Extensions.Options;

namespace LocadoraDeAutomoveis.Application.Auth.Services;

public class AuthEmailService(
    IEmailSender emailSender,
    IEmailTemplateService templateService,
    IOptions<AppUrlsOptions> appUrlsOptions
) : IAuthEmailService
{
    private readonly AppUrlsOptions appUrls = appUrlsOptions.Value;

    public async Task SendForgotPasswordEmailAsync(string email, string userName, string resetToken, bool toPortal, string? language = null)
    {
        string resolvedLanguage = language ?? "pt-BR";

        string encodedToken = Uri.EscapeDataString(resetToken);
        string encodedEmail = Uri.EscapeDataString(email);

        string baseUrl = toPortal ? this.appUrls.PortalApp : this.appUrls.AdminApp;
        string resetPasswordUrl = $"{baseUrl}/auth/reset-password?token={encodedToken}&email={encodedEmail}";

        Dictionary<string, string> placeholders = new()
        {
            {"UserName", userName },
            {"Link", resetPasswordUrl }
        };

        string body = await templateService.GetTemplateAsync("forget-password", placeholders, resolvedLanguage);
        string subject = GetSubject("forget-password", resolvedLanguage);

        BackgroundJob.Enqueue(() => emailSender.SendAsync(email, subject, body));
    }

    public async Task ScheduleClientRegisterWelcome(string email, string fullName, string? language = null)
    {
        string resolvedLanguage = language ?? "pt-BR";

        Dictionary<string, string> placeholders = new()
        {
            {"UserFullName", fullName },
            { "LoginUrl", $"{this.appUrls.PortalApp}/home" }
        };

        string body = await templateService.GetTemplateAsync("welcome-client", placeholders, resolvedLanguage);
        string subject = GetSubject("welcome-client", resolvedLanguage);

        BackgroundJob.Enqueue(() => emailSender.SendAsync(email, subject, body));
    }

    public async Task ScheduleClientGoogleWelcome(string email, string fullName, string resetToken, string? language = null)
    {
        string resolvedLanguage = language ?? "pt-BR";

        string encodedToken = Uri.EscapeDataString(resetToken);
        string encodedEmail = Uri.EscapeDataString(email);

        string resetPasswordUrl = $"{this.appUrls.PortalApp}/auth/reset-password?token={encodedToken}&email={encodedEmail}";

        Dictionary<string, string> placeholders = new()
        {
            {"UserFullName", fullName },
            {"ResetPasswordUrl", resetPasswordUrl}
        };

        string body = await templateService.GetTemplateAsync("welcome-client-google", placeholders, resolvedLanguage);
        string subject = GetSubject("welcome-client-google", resolvedLanguage);

        BackgroundJob.Enqueue(() => emailSender.SendAsync(email, subject, body));
    }

    public async Task ScheduleBusinessRegisterWelcome(string email, string fullName, string? language = null)
    {
        string resolvedLanguage = language ?? "pt-BR";

        Dictionary<string, string> placeholders = new()
        {
            {"UserFullName", fullName },
            {"LoginUrl", $"{this.appUrls.AdminApp}/home" }
        };

        string body = await templateService.GetTemplateAsync("welcome-business", placeholders, resolvedLanguage);
        string subject = GetSubject("welcome-business", resolvedLanguage);

        BackgroundJob.Enqueue(() => emailSender.SendAsync(email, subject, body));
    }

    public async Task ScheduleBusinessGoogleWelcome(string email, string fullName, string resetToken, string? language = null)
    {
        string resolvedLanguage = language ?? "pt-BR";

        string encodedToken = Uri.EscapeDataString(resetToken);
        string encodedEmail = Uri.EscapeDataString(email);

        string resetPasswordUrl = $"{this.appUrls.AdminApp}/auth/reset-password?token={encodedToken}&email={encodedEmail}";

        Dictionary<string, string> placeholders = new()
        {
            {"UserFullName", fullName },
            {"ResetPasswordUrl", resetPasswordUrl }
        };

        string body = await templateService.GetTemplateAsync("welcome-business-google", placeholders, resolvedLanguage);
        string subject = GetSubject("welcome-business-google", resolvedLanguage);

        BackgroundJob.Enqueue(() => emailSender.SendAsync(email, subject, body));
    }

    public async Task ScheduleClientInvitation(string email, string fullName, string resetToken, string? language = null)
    {
        string resolvedLanguage = language ?? "pt-BR";

        string encodedToken = Uri.EscapeDataString(resetToken);
        string encodedEmail = Uri.EscapeDataString(email);

        string resetPasswordUrl = $"{this.appUrls.PortalApp}/auth/reset-password?token={encodedToken}&email={encodedEmail}";

        Dictionary<string, string> placeholders = new()
        {
            {"ClientName", fullName },
            {"ResetPasswordUrl", resetPasswordUrl }
        };

        string body = await templateService.GetTemplateAsync("client-invitation", placeholders, resolvedLanguage);
        string subject = GetSubject("client-invitation", resolvedLanguage);

        BackgroundJob.Enqueue(() => emailSender.SendAsync(email, subject, body));
    }

    public async Task ScheduleAccountDeactivationNotice(string email, string fullName, string? language = null)
    {
        string resolvedLanguage = language ?? "pt-BR";

        Dictionary<string, string> placeholders = new()
        {
            {"UserFullName", fullName },
        };

        string body = await templateService.GetTemplateAsync("account-deactivated", placeholders, resolvedLanguage);
        string subject = GetSubject("account-deactivated", resolvedLanguage);

        BackgroundJob.Enqueue(() => emailSender.SendAsync(email, subject, body));
    }

    private static string GetSubject(string templateKey, string language)
    {
        return (templateKey, language) switch
        {
            ("forget-password", "pt-BR") => "Recuperação de senha - LDA",
            ("forget-password", "es-ES") => "Recuperación de contraseña - LDA",
            ("forget-password", _) => "Password Recovery - LDA",

            ("welcome-client", "pt-BR") => "Bem-vindo! - LDA",
            ("welcome-client", "es-ES") => "¡Bienvenido! - LDA",
            ("welcome-client", _) => "Welcome! - LDA",

            ("welcome-client-google", "pt-BR") => "Cadastro com Google - LDA",
            ("welcome-client-google", "es-ES") => "Registro con Google - LDA",
            ("welcome-client-google", _) => "Google Registration - LDA",

            ("welcome-business", "pt-BR") => "Bem-vindo, parceiro! - LDA",
            ("welcome-business", "es-ES") => "¡Bienvenido, socio! - LDA",
            ("welcome-business", _) => "Welcome Partner - LDA",

            ("welcome-business-google", "pt-BR") => "Cadastro com Google - LDA",
            ("welcome-business-google", "es-ES") => "Registro con Google - LDA",
            ("welcome-business-google", _) => "Google Registration - LDA",

            ("client-invitation", "pt-BR") => "Bem-vindo à família - LDA",
            ("client-invitation", "es-ES") => "Bienvenido a la familia - LDA",
            ("client-invitation", _) => "Welcome to the Family - LDA",

            ("account-deactivated", "pt-BR") => "Aviso importante sobre sua conta - LDA",
            ("account-deactivated", "es-ES") => "Aviso importante sobre su cuenta - LDA",
            ("account-deactivated", _) => "Important notice regarding your account - LDA",

            _ => "LDA"
        };
    }
}
