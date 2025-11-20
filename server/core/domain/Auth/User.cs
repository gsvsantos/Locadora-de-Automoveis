using Microsoft.AspNetCore.Identity;

namespace LocadoraDeAutomoveis.Domain.Auth;

public class User : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;

    public User()
    {
        this.Id = Guid.NewGuid();
        this.EmailConfirmed = true;
        this.PhoneNumberConfirmed = true;
    }
}
