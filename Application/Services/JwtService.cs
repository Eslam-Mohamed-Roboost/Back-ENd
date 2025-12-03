using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace API.Application.Services;

public class JwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string? GenerateAccessToken(long jwtID, string userId, string roleID)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);
        
        if (key.Length < 32)
        {
            throw new InvalidOperationException("JWT Secret key must be at least 32 bytes (256 bits)");
        }

        var signingKey = new SymmetricSecurityKey(key);
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(ClaimTypes.Role, roleID),
            new Claim(JwtRegisteredClaimNames.Jti, jwtID.ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["TokenExpiryMinutes"]!)),
            SigningCredentials = signingCredentials,
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            NotBefore = DateTime.UtcNow
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}

