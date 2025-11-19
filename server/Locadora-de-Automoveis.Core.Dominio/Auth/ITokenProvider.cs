namespace Locadora_de_Automoveis.Core.Dominio.Auth;

public interface ITokenProvider
{
    IAccessToken GetAccessToken(User user);
}
