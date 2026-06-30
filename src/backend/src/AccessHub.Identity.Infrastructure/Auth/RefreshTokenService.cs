using System.Security.Cryptography;
using System.Text;
using AccessHub.Identity.Application.Auth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace AccessHub.Identity.Infrastructure.Auth;

public sealed class RefreshTokenService(IConfiguration configuration) : IRefreshTokenService
{
  public GeneratedRefreshToken GenerateToken()
  {
    var lifetimeDays = configuration.GetValue<int?>("Jwt:RefreshTokenLifetimeDays")
      ?? throw new InvalidOperationException("Jwt:RefreshTokenLifetimeDays is required.");

    if (lifetimeDays <= 0)
    {
      throw new InvalidOperationException("Jwt:RefreshTokenLifetimeDays must be greater than zero.");
    }

    var token = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
    var tokenHash = HashToken(token);

    return new GeneratedRefreshToken(token, tokenHash, DateTime.UtcNow.AddDays(lifetimeDays));
  }

  public string HashToken(string token)
  {
    return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
  }
}
