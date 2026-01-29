using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookHub.UserService.Domain.Entities;
using BookHub.UserService.Domain.Ports;
using Microsoft.IdentityModel.Tokens;

namespace BookHub.UserService.Infrastructure.Security;

public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTime ExpiresAt) GenerateToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "BookHubSuperSecretKeyThatIsAtLeast32CharactersLong!";
        var issuer = jwtSettings["Issuer"] ?? "BookHub";
        var audience = jwtSettings["Audience"] ?? "BookHubUsers";
        var expirationHours = int.Parse(jwtSettings["ExpirationHours"] ?? "24");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expiresAt = DateTime.UtcNow.AddHours(expirationHours);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
