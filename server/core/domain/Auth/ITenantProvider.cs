namespace LocadoraDeAutomoveis.Domain.Auth;

public interface ITenantProvider
{
    Guid? TenantId { get; }
    Guid GetTenantId()
    {
        return this.TenantId.GetValueOrDefault();
    }
}