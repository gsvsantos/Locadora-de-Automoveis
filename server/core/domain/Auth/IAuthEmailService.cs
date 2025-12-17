namespace LocadoraDeAutomoveis.Domain.Auth;

public interface IAuthEmailService
{
    Task ScheduleBusinessRegisterWelcome(User user);
    Task ScheduleBusinessGoogleLoginWelcome(User user, string resetToken);
    Task ScheduleClientInvitation(User user, string resetToken);
}
