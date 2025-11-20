using LocadoraDeAutomoveis.Domain.Auth;

namespace LocadoraDeAutomoveis.Domain.Shared;

public abstract class BaseEntity<Tipo>
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public User? Tenant { get; set; }

    protected BaseEntity() => this.Id = Guid.NewGuid();

    public abstract void Update(Tipo registroEditado);
}
