using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace LocadoraDeAutomoveis.WebApi.Filters;

public sealed class HangfireDashboardPlatformAdminFilter : IDashboardAuthorizationFilter
{
    private const string PlatformAdminRoleName = "PlatformAdmin";

    public bool Authorize(DashboardContext context)
    {
        HttpContext httpContext = context.GetHttpContext();

        AuthenticateResult cookieAuthResult =
            httpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme)
                       .GetAwaiter().GetResult();

        if (!cookieAuthResult.Succeeded || cookieAuthResult.Principal is null)
        {
            return false;
        }

        return cookieAuthResult.Principal.IsInRole(PlatformAdminRoleName);
    }
}