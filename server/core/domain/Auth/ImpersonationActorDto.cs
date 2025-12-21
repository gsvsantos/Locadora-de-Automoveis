namespace LocadoraDeAutomoveis.Domain.Auth;

public sealed record ImpersonationActorDto(
    Guid ActorUserId,
    Guid ActorTenantId,
    string ActorUserName,
    string ActorEmail
);