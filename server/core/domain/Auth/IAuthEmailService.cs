namespace LocadoraDeAutomoveis.Domain.Auth;

public interface IAuthEmailService
{
    Task SendForgotPasswordEmailAsync(string email, string userName, string resetToken);
    Task ScheduleClientRegisterWelcome(string email, string fullName);
    Task ScheduleClientGoogleWelcome(string email, string fullName, string resetToken);
    Task ScheduleBusinessRegisterWelcome(string email, string fullName);
    Task ScheduleBusinessGoogleWelcome(string email, string fullName, string resetToken);
    Task ScheduleClientInvitation(string email, string fullName, string resetToken);
}
