using System.ComponentModel.DataAnnotations;

namespace AccessHub.Identity.Api.Configuration;

public sealed class JwtOptions
{
  public const string SectionName = "Jwt";

  [Required]
  public string Issuer { get; init; } = string.Empty;

  [Required]
  public string Audience { get; init; } = string.Empty;

  [Required]
  [MinLength(32)]
  public string SigningKey { get; init; } = string.Empty;

  [Range(1, int.MaxValue)]
  public int AccessTokenLifetimeMinutes { get; init; } = 15;

  [Range(1, int.MaxValue)]
  public int RefreshTokenLifetimeDays { get; init; } = 7;
}
