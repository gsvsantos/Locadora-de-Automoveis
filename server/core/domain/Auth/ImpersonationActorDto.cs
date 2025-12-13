namespace LocadoraDeAutomoveis.Application.Admin.Commands.DTOs;

public sealed record ImpersonationActorDto(
    Guid ActorUserId,
    Guid ActorTenantId,
    string ActorUserName,
    string ActorEmail
);