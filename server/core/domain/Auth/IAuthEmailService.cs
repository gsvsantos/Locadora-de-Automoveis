namespace LocadoraDeAutomoveis.Domain.Auth;

public interface IAuthEmailService
{
    Task SendForgotPasswordEmailAsync(string email, string userName, string resetToken);
    Task ScheduleBusinessRegisterWelcome(string email, string fullName);
    Task ScheduleBusinessGoogleLoginWelcome(string email, string fullName, string resetToken);
    Task ScheduleClientInvitation(string email, string fullName, string resetToken);
}
