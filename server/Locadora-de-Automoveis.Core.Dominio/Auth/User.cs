using Microsoft.AspNetCore.Identity;

namespace Locadora_de_Automoveis.Core.Dominio.Auth;

public class User : IdentityUser<Guid>
{
    public string FullName { get; set; }

    public User()
    {
        this.Id = Guid.NewGuid();
        this.EmailConfirmed = true;
    }
}
