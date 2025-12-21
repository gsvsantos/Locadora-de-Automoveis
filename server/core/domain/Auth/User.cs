using Microsoft.AspNetCore.Identity;

namespace LocadoraDeAutomoveis.Domain.Auth;

public class User : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
    public Guid AccessTokenVersionId { get; set; } = Guid.Empty;
    public Guid? TenantId { get; set; }
    public string PreferredLanguage { get; private set; } = "pt-BR";

    public User()
    {
        this.Id = Guid.NewGuid();
        this.EmailConfirmed = true;
        this.PhoneNumberConfirmed = true;
    }

    public void AssociateTenant(Guid tenantId)
    {
        this.TenantId = tenantId == Guid.Empty ? null : tenantId;
    }

    public void SetPreferredLanguage(string language)
    {
        this.PreferredLanguage = language;
    }

    public Guid GetTenantId()
    {
        return this.TenantId.GetValueOrDefault();
    }
}
