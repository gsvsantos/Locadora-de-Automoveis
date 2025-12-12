using LocadoraDeAutomoveis.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Domain.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LocadoraDeAutomoveis.Application.Auth.Services;

public class JwtProvider : ITokenProvider
{
    private readonly UserManager<User> userManager;
    private readonly string? jwtKey;
    private readonly DateTime jwtExpiration;
    private readonly string? validAudience;

    public JwtProvider(IConfiguration configuration, UserManager<User> userManager)
    {
        this.jwtKey = configuration["JWT_GENERATION_KEY"];

        if (string.IsNullOrEmpty(this.jwtKey))
        {
            throw new ArgumentException("The environment variable \"JWT_GENERATION_KEY\" was not provided");
        }

        this.validAudience = configuration["JWT_AUDIENCE_DOMAIN"];

        if (string.IsNullOrEmpty(this.validAudience))
        {
            throw new ArgumentException("The environment variable \"JWT_AUDIENCE_DOMAIN\" was not provided");
        }

        this.jwtExpiration = DateTime.UtcNow.AddMinutes(5);

        this.userManager = userManager;
    }

    public async Task<IAccessToken> GenerateAccessToken(User user)
    {
        byte[] keyBytes = Encoding.ASCII.GetBytes(this.jwtKey!);

        IList<string> userRoles = await this.userManager.GetRolesAsync(user);

        string? domainRoleName = (
                userRoles.Contains(nameof(Roles.Admin), StringComparer.OrdinalIgnoreCase) ? nameof(Roles.Admin) :
                userRoles.Contains(nameof(Roles.Employee), StringComparer.OrdinalIgnoreCase) ? nameof(Roles.Employee) :
                null)
            ?? throw new InvalidOperationException("User has no domain role (Admin/Employee).");

        Roles domainRole = Enum.Parse<Roles>(domainRoleName, true);

        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.TenantId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, user.AccessTokenVersionId.ToString()),
            new Claim("user_id", user.Id.ToString())
        ];

        foreach (string role in userRoles)
        {
            if (Enum.TryParse<Roles>(role, true, out _))
            {
                claims.Add(new Claim("roles", role));
            }
        }

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Issuer = "LocadoraDeAutomoveis",
            Audience = this.validAudience,
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature
            ),
            Expires = this.jwtExpiration,
            NotBefore = DateTime.UtcNow
        };

        JwtSecurityTokenHandler tokenHandler = new();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        string tokenString = tokenHandler.WriteToken(token);

        return new AccessToken()
        {
            Key = tokenString,
            Expiration = this.jwtExpiration,
            User = new UserAuthenticatedDto
            {
                Id = user.Id,
                FullName = user.FullName ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Role = domainRole
            }
        };
    }
}