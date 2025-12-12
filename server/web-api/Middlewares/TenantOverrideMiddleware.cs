using Microsoft.Extensions.Primitives;

namespace LocadoraDeAutomoveis.WebApi.Middlewares;

public sealed class TenantOverrideMiddleware(
    RequestDelegate next,
    ILogger<TenantOverrideMiddleware> logger
)
{
    public const string TenantOverrideHeaderName = "X-Tenant-Override";

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (!httpContext.Request.Headers.TryGetValue(TenantOverrideHeaderName, out StringValues values))
        {
            await next(httpContext);
            return;
        }

        string rawTenantId = values.ToString();
        if (!Guid.TryParse(rawTenantId, out Guid overrideTenantId))
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsync("Invalid X-Tenant-Override value.");
            return;
        }

        if (overrideTenantId == Guid.Empty)
        {
            await next(httpContext);
            return;
        }

        if (!httpContext.User.Identity?.IsAuthenticated ?? true)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        if (!httpContext.User.IsInRole("PlatformAdmin"))
        {
            logger.LogWarning("Security Alert: User '{User}' tried to impersonate Tenant '{Tenant}' but is not PlatformAdmin.",
                httpContext.User.Identity?.Name, overrideTenantId);

            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            await httpContext.Response.WriteAsync("Tenant override is only allowed for PlatformAdmin.");
            return;
        }

        httpContext.Items[TenantOverrideHeaderName] = overrideTenantId;

        logger.LogWarning(
            "Tenant override: user '{UserName}' (user_id: {UserId}) acting as tenant {TenantId} on {Method} {Path}.",
            httpContext.User.Identity?.Name,
            httpContext.User.FindFirst("user_id")?.Value,
            overrideTenantId,
            httpContext.Request.Method,
            httpContext.Request.Path
        );

        await next(httpContext);
    }
}