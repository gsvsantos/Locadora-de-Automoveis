using System.Collections.Immutable;

namespace LocadoraDeAutomoveis.Application.Admin.Commands;

public record GetAllTenantsResponse(
    int Quantity,
    ImmutableList<TenantDto> Tenants
);

public record TenantDto(
    Guid TenantId,
    Guid AdminUserId,
    string DisplayName,
    string Email,
    IList<string> Roles
);