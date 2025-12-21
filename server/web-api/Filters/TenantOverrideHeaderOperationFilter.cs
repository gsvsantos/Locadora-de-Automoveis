using LocadoraDeAutomoveis.WebApi.Middlewares;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LocadoraDeAutomoveis.WebApi.Filters;

public sealed class TenantOverrideHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation op, OperationFilterContext ctx)
    {
        string route = ctx.ApiDescription.RelativePath?.ToLowerInvariant() ?? string.Empty;

        bool shouldNotOverride =
            route.StartsWith("api/auth") ||
            route.StartsWith("api/search") ||
            route.StartsWith("api/admin");

        if (shouldNotOverride)
        {
            return;
        }

        op.Parameters ??= [];

        op.Parameters.Add(new OpenApiParameter
        {
            Name = TenantOverrideMiddleware.TenantOverrideHeaderName,
            In = ParameterLocation.Header,
            Required = false,
            Description = "Tenant ID for impersonation (PlatformAdmin only). Leave as default (0000...) to ignore.",
            Schema = new OpenApiSchema
            {
                Type = "string",
                Format = "uuid"
            },
            Example = new Microsoft.OpenApi.Any.OpenApiString("00000000-0000-0000-0000-000000000000")
        });
    }
}