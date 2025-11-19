using LocadoraDeAutomoveis.Core.Application.Auth.DTOs;
using LocadoraDeAutomoveis.Core.Domain.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LocadoraDeAutomoveis.Core.Application.Auth.Services;

public class JwtProvider : ITokenProvider
{
    private readonly UserManager<User> userManager;
    private readonly string? jwtKey;
    private readonly DateTime jwtExpiration;
    private readonly string? validAudience;

    public JwtProvider(IConfiguration configutarion, UserManager<User> userManager)
    {
        this.jwtKey = configutarion["JWT_GENERATION_KEY"];

        if (string.IsNullOrEmpty(this.jwtKey))
        {
            throw new ArgumentException("The environment variable \"JWT_GENERATION_KEY\" was not provided");
        }

        this.validAudience = configutarion["JWT_AUDIENCE_DOMAIN"];

        if (string.IsNullOrEmpty(this.validAudience))
        {
            throw new ArgumentException("The environment variable \"JWT_AUDIENCE_DOMAIN\" was not provided");
        }

        this.jwtExpiration = DateTime.UtcNow.AddMinutes(5);

        this.userManager = userManager;
    }

    public async Task<IAccessToken> GenerateAccessToken(User user)
    {
        JwtSecurityTokenHandler tokenHandler = new();

        byte[] keyBytes = Encoding.ASCII.GetBytes(this.jwtKey!);

        IList<string> userRoles = await this.userManager.GetRolesAsync(user);

        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!)
        ];

        foreach (string role in userRoles)
        {
            claims.Add(new Claim("roles", role));
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

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        string tokenString = tokenHandler.WriteToken(token);

        return new TokenResponse()
        {
            Key = tokenString,
            Expiration = this.jwtExpiration,
            User = new UserAuthenticatedDto
            {
                Id = user.Id,
                FullName = user.FullName ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty
            }
        };
    }
}