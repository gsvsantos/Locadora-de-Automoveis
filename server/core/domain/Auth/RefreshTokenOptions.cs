namespace LocadoraDeAutomoveis.Domain.Auth;

public class RefreshTokenOptions
{
    public int ExpirationInDays { get; set; } = 7;
    public string PepperSecret { get; set; } = string.Empty;
    public string CookieName { get; set; } = "LocadoraDeAutomoveis.RefreshToken";
}
