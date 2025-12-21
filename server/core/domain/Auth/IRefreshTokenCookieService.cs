using Microsoft.AspNetCore.Http;

namespace LocadoraDeAutomoveis.Domain.Auth;

public interface IRefreshTokenCookieService
{
    void Write(HttpResponse response, IssuedRefreshTokenDto refreshToken);
    string? Get(HttpRequest request);
    void Remove(HttpResponse response);
}
