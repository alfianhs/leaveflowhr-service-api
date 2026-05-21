using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LeaveFlowHR.Api.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LeaveFlowHR.Api.Infrastructure.Services;

public interface IJwtService
{
    string GenerateToken(Guid userId, string role, DateTime generatedAt);
    DateTime GetTokenExpiredAt(DateTime generatedAt);
}

public class JwtService : IJwtService
{
    private readonly AppConfiguration _appConfiguration;

    public JwtService(IOptions<AppConfiguration> appConfiguration)
    {
        _appConfiguration = appConfiguration.Value;
    }

    public string GenerateToken(Guid userId, string role, DateTime generatedAt)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfiguration.Jwt.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _appConfiguration.Jwt.Issuer,
            audience: _appConfiguration.Jwt.Audience,
            claims: claims,
            expires: generatedAt.AddMinutes(_appConfiguration.Jwt.ExpiryInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public DateTime GetTokenExpiredAt(DateTime generatedAt)
    {
        return generatedAt.AddMinutes(_appConfiguration.Jwt.ExpiryInMinutes);
    }
}