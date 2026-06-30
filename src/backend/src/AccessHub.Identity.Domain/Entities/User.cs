namespace AccessHub.Identity.Domain.Entities;

public sealed class User
{
  public Guid Id { get; set; }

  public string Email { get; set; } = string.Empty;

  public string Username { get; set; } = string.Empty;

  public string PasswordHash { get; set; } = string.Empty;

  public bool IsEnabled { get; set; }

  public bool IsLocked { get; set; }

  public DateTime CreatedAtUtc { get; set; }

  public DateTime UpdatedAtUtc { get; set; }

  public ICollection<UserRole> UserRoles { get; set; } = [];

  public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
