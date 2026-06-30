using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AccessHub.Identity.Application.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AccessHub.Identity.Infrastructure.Auth;

public sealed class AccessTokenService(IConfiguration configuration) : IAccessTokenService
{
  public GeneratedAccessToken GenerateToken(AuthenticatedUser user)
  {
    var issuer = configuration["Jwt:Issuer"];
    var audience = configuration["Jwt:Audience"];
    var signingKey = configuration["Jwt:SigningKey"];
    var lifetimeMinutes = configuration.GetValue<int?>("Jwt:AccessTokenLifetimeMinutes");

    if (string.IsNullOrWhiteSpace(issuer))
    {
      throw new InvalidOperationException("Jwt:Issuer is required.");
    }

    if (string.IsNullOrWhiteSpace(audience))
    {
      throw new InvalidOperationException("Jwt:Audience is required.");
    }

    if (string.IsNullOrWhiteSpace(signingKey))
    {
      throw new InvalidOperationException("Jwt:SigningKey is required.");
    }

    if (lifetimeMinutes is null || lifetimeMinutes <= 0)
    {
      throw new InvalidOperationException("Jwt:AccessTokenLifetimeMinutes must be greater than zero.");
    }

    var now = DateTime.UtcNow;
    var expiresAtUtc = now.AddMinutes(lifetimeMinutes.Value);
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
      new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
      new Claim(JwtRegisteredClaimNames.Email, user.Email),
      new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var token = new JwtSecurityToken(
      issuer,
      audience,
      claims,
      now,
      expiresAtUtc,
      credentials);

    return new GeneratedAccessToken(new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
  }
}
