using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.WebApi.Middlewares;
using System.Security.Claims;

namespace LocadoraDeAutomoveis.WebAPI.Identity;

public sealed class IdentityTenantProvider(IHttpContextAccessor contextAccessor) : ITenantProvider, IUserContext
{
    private const string TenantOverrideKey = TenantOverrideMiddleware.TenantOverrideHeaderName;

    public Guid? TenantId
    {
        get
        {
            ClaimsPrincipal? user = contextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            if (contextAccessor.HttpContext!.User.IsInRole("PlatformAdmin") &&
                contextAccessor.HttpContext!.Items.TryGetValue(TenantOverrideKey, out object? value) &&
                value is Guid overrideTenantId)
            {
                return overrideTenantId;
            }

            Claim? tenantClaim = user.FindFirst("sub");

            if (tenantClaim == null)
            {
                return null;
            }

            return TryParseGuid(tenantClaim.Value);
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
