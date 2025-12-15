namespace LocadoraDeAutomoveis.Domain.Auth;

public interface IRecaptchaService
{
    Task<bool> VerifyRecaptchaToken(string token);
}
