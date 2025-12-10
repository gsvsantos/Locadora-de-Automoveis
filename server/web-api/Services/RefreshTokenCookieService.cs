using LocadoraDeAutomoveis.Domain.Auth;

namespace LocadoraDeAutomoveis.WebApi.Services;

public sealed class RefreshTokenCookieService(RefreshTokenOptions refreshTokenOptions)
    : IRefreshTokenCookieService
{
    private readonly string cookieName = refreshTokenOptions.CookieName;
    private const string RefreshTokenPath = "/api/auth";
    public void Write(HttpResponse response, RefreshToken refreshToken)
    {
        CookieOptions options = new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = RefreshTokenPath,
            Expires = refreshToken.ExpirationDateUtc,
            IsEssential = true
        };

        response.Cookies.Append(this.cookieName, refreshToken.TokenHash, options);
    }

    public string? Get(HttpRequest request)
    {
        return request.Cookies.TryGetValue(this.cookieName, out string? value) ? value : null;
    }

    public void Remove(HttpResponse response)
    {
        response.Cookies.Delete(this.cookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = RefreshTokenPath
        });
    }
}