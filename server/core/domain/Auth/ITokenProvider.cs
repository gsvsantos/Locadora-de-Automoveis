using LocadoraDeAutomoveis.Application.Admin.Commands.DTOs;

namespace LocadoraDeAutomoveis.Domain.Auth;

public interface ITokenProvider
{
    Task<IAccessToken> GenerateAccessToken(User user, ImpersonationActorDto? actor = null);
}
