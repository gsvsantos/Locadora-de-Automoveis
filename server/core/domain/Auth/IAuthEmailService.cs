namespace LocadoraDeAutomoveis.Domain.Auth;

public interface IAuthEmailService
{
    Task SendForgotPasswordEmailAsync(string email, string userName, string resetToken, bool isPortal, string? language = null);
    Task ScheduleClientRegisterWelcome(string email, string fullName, string? language = null);
    Task ScheduleClientGoogleWelcome(string email, string fullName, string resetToken, string? language = null);
    Task ScheduleBusinessRegisterWelcome(string email, string fullName, string? language = null);
    Task ScheduleBusinessGoogleWelcome(string email, string fullName, string resetToken, string? language = null);
    Task ScheduleClientInvitation(string email, string fullName, string resetToken, string? language = null);
}
