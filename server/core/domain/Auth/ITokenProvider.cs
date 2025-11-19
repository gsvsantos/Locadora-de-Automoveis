namespace LocadoraDeAutomoveis.Core.Domain.Auth;

public interface ITokenProvider
{
    Task<IAccessToken> GenerateAccessToken(User user);
}
