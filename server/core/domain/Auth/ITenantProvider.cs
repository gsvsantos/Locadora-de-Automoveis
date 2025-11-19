namespace LocadoraDeAutomoveis.Core.Domain.Auth;

public interface ITenantProvider
{
    Guid? UserId { get; }
    Guid GetUserId() => this.UserId.GetValueOrDefault();
}