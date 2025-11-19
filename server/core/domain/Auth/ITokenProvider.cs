namespace LocadoraDeAutomoveis.Core.Domain.Auth;

public interface ITokenProvider
{
    IAccessToken GetAccessToken(User user);
}
