namespace LocadoraDeAutomoveis.Domain.Auth;

public interface ITenantProvider
{
    Guid? TenantId { get; }
    Guid GetTenantId() => this.TenantId.GetValueOrDefault();
}