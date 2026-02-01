using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BookHub.LoanService.Infrastructure.Security;

// Génère un JWT technique pour les appels inter-services
public class InternalJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public InternalJwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Generate()
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]!;
        var issuer = jwtSettings["Issuer"]!;
        var audience = jwtSettings["Audience"]!;

        var claims = new[]
        {
            // IMPORTANT : correspond à la policy "InternalService"
            new Claim("scope", "internal")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
