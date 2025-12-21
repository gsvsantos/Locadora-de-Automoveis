namespace LocadoraDeAutomoveis.Domain.Auth;

public interface ITokenProvider
{
    Task<IAccessToken> GenerateAccessToken(User user, ImpersonationActorDto? actor = null);
}
