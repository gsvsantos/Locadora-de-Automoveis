using LocadoraDeAutomoveis.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Domain.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace LocadoraDeAutomoveis.Application.Auth.Services;

public class JwtProvider : ITokenProvider
{
    private readonly UserManager<User> userManager;
    private readonly string? jwtKey;
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

        this.userManager = userManager;
    }

    public async Task<IAccessToken> GenerateAccessToken(User user, ImpersonationActorDto? actor = null)
    {
        DateTime jwtExpiration = DateTime.UtcNow.AddMinutes(30);

        byte[] keyBytes = Encoding.ASCII.GetBytes(this.jwtKey!);

        IList<string> userRoles = await this.userManager.GetRolesAsync(user);

        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, user.AccessTokenVersionId.ToString()),
            new Claim("tenant_id", user.TenantId.ToString()),
        ];

        foreach (string role in userRoles)
        {
            claims.Add(new Claim("roles", role));
        }

        if (actor is not null)
        {
            claims.Add(new Claim("impersonation", "true"));

            string actorJson = JsonSerializer.Serialize(new
            {
                sub = actor.ActorUserId,
                tid = actor.ActorTenantId,
                email = actor.ActorEmail,
                uname = actor.ActorUserName
            });

            claims.Add(new Claim("act", actorJson, JsonClaimValueTypes.Json));
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
            Expires = jwtExpiration,
            NotBefore = DateTime.UtcNow
        };

        JwtSecurityTokenHandler tokenHandler = new();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        string tokenString = tokenHandler.WriteToken(token);

        return new AccessToken()
        {
            Key = tokenString,
            Expiration = jwtExpiration,
            User = new UserAuthenticatedDto
            {
                Id = user.Id,
                FullName = user.FullName ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Roles = userRoles.ToList()
            }
        };
    }
}