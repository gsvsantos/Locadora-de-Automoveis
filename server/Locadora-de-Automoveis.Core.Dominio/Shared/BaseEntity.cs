namespace Locadora_de_Automoveis.Core.Dominio.Shared;

public abstract class BaseEntity<Tipo>
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }

    public void VincularTenant(Guid tenantId) => this.TenantId = tenantId;

    public abstract void AtualizarRegistro(Tipo registroEditado);
}
