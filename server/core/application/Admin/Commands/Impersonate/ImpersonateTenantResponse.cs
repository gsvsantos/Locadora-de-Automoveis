using LocadoraDeAutomoveis.Application.Auth.DTOs;

namespace LocadoraDeAutomoveis.Application.Admin.Commands.Impersonate;

public record ImpersonateTenantResponse(
    AccessToken AccessToken
);
