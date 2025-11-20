using Microsoft.AspNetCore.Identity;

namespace LocadoraDeAutomoveis.Domain.Auth;

public class Role : IdentityRole<Guid>;

public enum Roles
{
    Admin,
    Employee
}
