using LocadoraDeAutomoveis.Domain.Auth;

namespace LocadoraDeAutomoveis.Application.Auth.DTOs;

public class TokenResponse : IAccessToken
{
    public required string Key { get; set; }
    public required DateTimeOffset Expiration { get; set; }
    public required UserAuthenticatedDto User { get; set; }
}