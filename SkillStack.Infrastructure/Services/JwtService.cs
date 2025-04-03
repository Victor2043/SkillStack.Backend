using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SkillStack.Core.Interfaces;
using SkillStack.Domain.Entities;
using SkillStack.Infrastructure.Persistence;

namespace SkillStack.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;
    private static readonly Dictionary<string, string> _refreshTokens = new();
    private readonly string _secretKey;

    public JwtService(IConfiguration configuration, AppDbContext context)
    {
        _configuration = configuration;
        _secretKey = configuration["Jwt:Secret"] ?? throw new ArgumentNullException("Jwt:Secret is missing");
        _context = context;
    }

    public async Task<string> GenerateAccessToken(Guid userId, string userName, string email)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, userName),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public bool ValidateRefreshToken(string refreshToken) => _refreshTokens.Values.Contains(refreshToken);

    public static void StoreRefreshToken(string userId, string refreshToken)
    {
        _refreshTokens[userId] = refreshToken;
    }

    public static string? GetUserIdByRefreshToken(string refreshToken)
    {
        return _refreshTokens.FirstOrDefault(x => x.Value == refreshToken).Key;
    }

    public async Task<string> GenerateActivationToken(Guid userId)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);
        var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, userId.ToString()),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);

        var tokenString = tokenHandler.WriteToken(securityToken);

        await _context.ActivationTokens.AddAsync(new ActivationToken
        {
            Token = tokenString,  
            UserId = userId,
            Expiration = DateTime.UtcNow.AddHours(24)
        });

        await _context.SaveChangesAsync();

        return tokenString;
    }

    public async Task<Guid?> ValidateActivationToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);

        try
        {
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, parameters, out var validatedToken);
            var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub);

            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
        }
        catch
        {
            return null;
        }
    }
}
