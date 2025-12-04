using LocadoraDeAutomoveis.Application.Auth.Services;
using LocadoraDeAutomoveis.Domain.Auth;
using LocadoraDeAutomoveis.Infrastructure.Shared;
using LocadoraDeAutomoveis.WebApi.Jobs;
using LocadoraDeAutomoveis.WebApi.Services;
using LocadoraDeAutomoveis.WebAPI.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace LocadoraDeAutomoveis.WebAPI;

public static class AuthDependencyInjection
{
    public static IServiceCollection ConfigureIdentityProviders(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<ITenantProvider, IdentityTenantProvider>();
        services.AddScoped<IUserContext, IdentityTenantProvider>();
        services.AddScoped<ITokenProvider, JwtProvider>();
        services.AddScoped<IRefreshTokenProvider, RefreshTokenProvider>();
        services.AddScoped<IRefreshTokenCookieService, RefreshTokenCookieService>();
        services.AddScoped<RefreshTokenCleanupJob>();

        services.AddIdentity<User, Role>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            }
        )
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        string? chaveAssinaturaJwt = configuration["JWT_GENERATION_KEY"]
            ?? throw new ArgumentException("The environment variable \"JWT_GENERATION_KEY\" was not provided.");

        byte[] chaveEmBytes = Encoding.ASCII.GetBytes(chaveAssinaturaJwt);

        string? audienciaValida = configuration["JWT_AUDIENCE_DOMAIN"]
            ?? throw new ArgumentException("The environment variable \"JWT_AUDIENCE_DOMAIN\" was not provided.");

        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.MapInboundClaims = false;
            x.RequireHttpsMetadata = true;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "LocadoraDeAutomoveis",
                ValidateAudience = true,
                ValidAudience = audienciaValida,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(chaveEmBytes),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(10),
                RoleClaimType = "roles",
                NameClaimType = "unique_name"
            };
            x.Events = new JwtBearerEvents
            {
                OnTokenValidated = async ctx =>
                {
                    string? userIdStr = ctx.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                    if (!Guid.TryParse(userIdStr, out Guid usuarioId))
                    {
                        ctx.Fail("Usuário inválido."); return;
                    }

                    UserManager<User> userManager = ctx.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
                    User? user = await userManager.FindByIdAsync(usuarioId.ToString());

                    if (user is null)
                    {
                        ctx.Fail("Versão do Access Token inválida.");
                    }
                }
            };
        });

        services.AddAuthorizationBuilder()
            .AddPolicy("AdminPolicy", p => p.RequireRole("Admin"))
            .AddPolicy("EmployeePolicy", p => p.RequireRole("Employee"))
            .AddPolicy("AdminOrEmployeePolicy", p => p.RequireRole("Admin", "Employee"));

        return services;
    }

    public static IServiceCollection ConfigureRefreshTokenOptions(this IServiceCollection services, IConfiguration configuration)
    {
        RefreshTokenOptions refreshTokenOptions = new();
        configuration.GetSection("RefreshTokenOptions").Bind(refreshTokenOptions);
        services.AddSingleton(refreshTokenOptions);

        return services;
    }
}
