using LocadoraDeAutomoveis.Domain.Auth;
using System.Security.Claims;

namespace LocadoraDeAutomoveis.WebAPI.Identity;

public sealed class IdentityTenantProvider(IHttpContextAccessor contextAccessor) : ITenantProvider, IUserContext
{
    public Guid? TenantId
    {
        get
        {
            ClaimsPrincipal? claimsPrincipal = contextAccessor.HttpContext?.User;

            if (claimsPrincipal?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            Claim? claimId = claimsPrincipal.FindFirst("sub");

            if (claimId == null)
            {
                return null;
            }

            return TryParseGuid(claimId.Value);
        }
    }

    public Guid? UserId
    {
        get
        {
            ClaimsPrincipal? claimsPrincipal = contextAccessor.HttpContext?.User;

            if (claimsPrincipal?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            Claim? claimId = claimsPrincipal.FindFirst("user_id");

            if (claimId == null)
            {
                return null;
            }

            return TryParseGuid(claimId.Value);
        }
    }

    public bool IsInRole(string roleName)
    {
        return contextAccessor.HttpContext?.User?.IsInRole(roleName) ?? false;
    }

    private static Guid? TryParseGuid(string? value)
    {
        if (Guid.TryParse(value, out Guid guid))
        {
            return guid;
        }

        return null;
    }
}
