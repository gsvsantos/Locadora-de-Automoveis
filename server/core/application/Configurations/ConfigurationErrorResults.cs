using FluentResults;

namespace LocadoraDeAutomoveis.Application.Configurations;

public abstract class ConfigurationErrorResults
{
    public static Error NotFoundForTenant(Guid tenantId)
    {
        return new Error("Configuration not found.")
            .CausedBy($"No configuration found for the tenant '{tenantId}'. Please perform the initial setup.")
            .WithMetadata("ErrorType", "NotFound");
    }
}
