using LocadoraDeAutomoveis.Domain.Auth;

namespace LocadoraDeAutomoveis.Domain.Shared;

public abstract class BaseEntity<T>
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public User? Tenant { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public bool IsActive { get; set; } = true;

    protected BaseEntity() => this.Id = Guid.NewGuid();

    public void Deactivate() => this.IsActive = false;

    public void AssociateUser(User user)
    {
        if (user is null)
        {
            return;
        }

        this.User = user;
        this.UserId = user.Id;
    }

    public void AssociateTenant(User tenant)
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

    public void AssociateTenant(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
        {
            return;
        }

        if (this.TenantId == tenantId && (this.Tenant is null || this.Tenant.Id == tenantId))
        {
            return;
        }

        this.TenantId = tenantId;

        if (this.Tenant is not null && this.Tenant.Id != tenantId)
        {
            this.Tenant = null;
        }
    }

    public abstract void Update(T updatedEntity);
}
