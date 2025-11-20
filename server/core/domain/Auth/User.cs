using Microsoft.AspNetCore.Identity;

namespace LocadoraDeAutomoveis.Domain.Auth;

public class User : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public Guid TenantId { get; set; }

    public User()
    {
        this.Id = Guid.NewGuid();
        this.EmailConfirmed = true;
        this.PhoneNumberConfirmed = true;
    }

    public void AssociateTenant(Guid tenantId) => this.TenantId = tenantId;
}
