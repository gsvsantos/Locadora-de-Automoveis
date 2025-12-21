namespace LocadoraDeAutomoveis.Domain.Auth;

public sealed record IssuedRefreshTokenDto(
    string PlainToken,
    DateTimeOffset ExpirationDateUtc
);
