using Microsoft.AspNetCore.Identity;

namespace Locadora_de_Automoveis.Core.Dominio.Auth;

public class Role : IdentityRole<Guid>;

public enum Roles
{
    Admin,
    Employee
}
