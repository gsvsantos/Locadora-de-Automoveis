using LocadoraDeAutomoveis.Domain.Auth;
using Microsoft.AspNetCore.Identity;

public static class SeedExtensions
{
    public static async Task IdentitySeederAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();

        ILogger logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("IdentitySeederAsync");

        IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        RoleManager<Role> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        foreach (string role in Enum.GetNames(typeof(Roles)))
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                IdentityResult createRole = await roleManager.CreateAsync(new Role { Name = role });

                if (!createRole.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Falha ao criar role '{role}': {string.Join("; ", createRole.Errors.Select(e => e.Description))}");
                }

                await roleManager.CreateAsync(new Role { Name = role });
            }
        }
    }
}
