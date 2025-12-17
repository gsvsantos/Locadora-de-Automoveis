namespace LocadoraDeAutomoveis.Domain.Auth;

public interface IAuthEmailService
{
    Task ScheduleGoogleLoginWelcome(User user);
}
