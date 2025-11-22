using LocadoraDeAutomoveis.Domain.Auth;

namespace LocadoraDeAutomoveis.Domain.Shared;

public abstract class BaseEntity<T>
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public User? Tenant { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public bool IsActive { get; set; } = true;

    protected BaseEntity() => this.Id = Guid.NewGuid();

    public abstract void Update(T updatedEntity);
}
