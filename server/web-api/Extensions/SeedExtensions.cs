using LocadoraDeAutomoveis.Domain.Auth;
using Microsoft.AspNetCore.Identity;

namespace LocadoraDeAutomoveis.WebApi.Extensions;

public static class SeedExtensions
{
    private const string PlatformAdminRoleName = "PlatformAdmin";
    private const string AdminRoleName = "Admin";

    public static async Task IdentitySeederAsync(this WebApplication app)
    {
        using IServiceScope serviceScope = app.Services.CreateScope();

        ILogger logger = serviceScope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(IdentitySeederAsync));

        IConfiguration configuration = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();

        string platformAdminFullName = GetRequiredConfig(configuration, "PLATFORM_ADMIN_FULLNAME");
        string platformAdminUserName = GetRequiredConfig(configuration, "PLATFORM_ADMIN_USERNAME");
        string platformAdminEmail = GetRequiredConfig(configuration, "PLATFORM_ADMIN_EMAIL");
        string platformAdminPassword = GetRequiredConfig(configuration, "PLATFORM_ADMIN_PASSWORD");

        RoleManager<Role> roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        UserManager<User> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();

        foreach (string domainRoleName in Enum.GetNames(typeof(Roles)))
        {
            if (await roleManager.RoleExistsAsync(domainRoleName))
            {
                continue;
            }

            IdentityResult createRoleResult = await roleManager.CreateAsync(new Role { Name = domainRoleName });
            if (!createRoleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create role '{domainRoleName}': {string.Join("; ", createRoleResult.Errors.Select(e => e.Description))}");
            }

            logger.LogInformation("Identity seed: role '{Role}' created.", domainRoleName);
        }

        if (!await roleManager.RoleExistsAsync(PlatformAdminRoleName))
        {
            IdentityResult createPlatformRoleResult = await roleManager.CreateAsync(new Role { Name = PlatformAdminRoleName });
            if (!createPlatformRoleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create role '{PlatformAdminRoleName}': {string.Join("; ", createPlatformRoleResult.Errors.Select(e => e.Description))}");
            }

            logger.LogInformation("Identity seed: role '{Role}' created.", PlatformAdminRoleName);
        }

        User? platformAdminUser = await userManager.FindByEmailAsync(platformAdminEmail);

        if (platformAdminUser is null)
        {
            platformAdminUser = new User
            {
                UserName = platformAdminUserName,
                Email = platformAdminEmail,
                FullName = platformAdminFullName,
                EmailConfirmed = true
            };
            platformAdminUser.AssociateTenant(platformAdminUser.Id);

            IdentityResult createUserResult = await userManager.CreateAsync(platformAdminUser, platformAdminPassword);
            if (!createUserResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create PlatformAdmin user: {string.Join("; ", createUserResult.Errors.Select(e => e.Description))}");
            }

            logger.LogInformation("Identity seed: PlatformAdmin user '{Email}' created.", platformAdminEmail);
        }
        else
        {
            logger.LogInformation("Identity seed: PlatformAdmin user '{Email}' already exists (password unchanged).", platformAdminEmail);
        }

        if (!await userManager.IsInRoleAsync(platformAdminUser, PlatformAdminRoleName) || !await userManager.IsInRoleAsync(platformAdminUser, AdminRoleName))
        {
            IdentityResult addRoleResult = await userManager.AddToRolesAsync(platformAdminUser, [PlatformAdminRoleName, AdminRoleName]);
            if (!addRoleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to grant role '{PlatformAdminRoleName}' to '{platformAdminEmail}': {string.Join("; ", addRoleResult.Errors.Select(e => e.Description))}");
            }

            logger.LogInformation(
                "Identity seed: roles '{Role1}', '{Role2}' granted to user '{Email}'.",
                PlatformAdminRoleName,
                AdminRoleName,
                platformAdminEmail
            );
        }
        else
        {
            logger.LogInformation(
                "Identity seed: user '{Email}' already has roles '{Role1}' and '{Role2}'.",
                platformAdminEmail,
                PlatformAdminRoleName,
                AdminRoleName
            );
        }
    }

    private static string GetRequiredConfig(IConfiguration configuration, string key)
    {
        string? value = configuration[key];
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Missing environment variable: {key}.");
        }

        return value;
    }
}
