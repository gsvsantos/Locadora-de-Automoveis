using Microsoft.AspNetCore.Identity;

namespace LocadoraDeAutomoveis.Core.Domain.Auth;

public class Role : IdentityRole<Guid>;

public enum Roles
{
    Admin,
    Employee
}
