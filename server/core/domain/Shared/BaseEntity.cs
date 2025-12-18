using LocadoraDeAutomoveis.Domain.Auth;

namespace LocadoraDeAutomoveis.Domain.Shared;

public abstract class BaseEntity<T>
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public User? Tenant { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    protected BaseEntity()
    {
        this.Id = Guid.NewGuid();
    }

    public virtual void Deactivate()
    {
        this.IsActive = false;
    }

    public virtual void AssociateUser(User user)
    {
        if (user is null)
        {
            return;
        }

        this.User = user;
        this.UserId = user.Id;
    }

    public virtual void AssociateTenant(User tenant)
    {
        if (tenant is null)
        {
            return;
        }

        if (this.Tenant is not null && this.Tenant.Id == tenant.Id)
        {
            return;
        }

        this.Tenant = tenant;
        this.TenantId = tenant.Id;
    }

    public virtual void AssociateTenant(Guid tenantId)
    {
        Guid? targetId = tenantId == Guid.Empty ? null : tenantId;

        if (this.TenantId == targetId)
        {
            return;
        }

        this.TenantId = targetId;

        if (this.Tenant is not null && this.Tenant.Id != targetId)
        {
            this.Tenant = null;
        }
    }

    public virtual Guid GetTenantId()
    {
        return this.TenantId.GetValueOrDefault();
    }

    public abstract void Update(T updatedEntity);
}
